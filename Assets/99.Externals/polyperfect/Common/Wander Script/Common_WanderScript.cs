using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AI;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Polyperfect.Common
{
    [RequireComponent(typeof(Animator)), RequireComponent(typeof(CharacterController))]
    public class Common_WanderScript : MonoBehaviour
    {
        private const float contingencyDistance = 1f;

        [SerializeField] public IdleState[] idleStates;
        [SerializeField] private MovementState[] movementStates;
        [SerializeField] private AIState[] attackingStates; 
        [SerializeField] private AIState[] deathStates;

        [SerializeField] public string species = "NA";

        [SerializeField, Tooltip("This specific animal stats asset, create a new one from the asset menu under (LowPolyAnimals/NewAnimalStats)")]
        public AIStats stats;

        [SerializeField, Tooltip("How far away from it's origin this animal will wander by itself.")]
        private float wanderZone = 10f;

        public float MaxDistance
        {
            get { return wanderZone; }
            set
            {
#if UNITY_EDITOR
                SceneView.RepaintAll();
#endif
                wanderZone = value;
            }
        }

        // [SerializeField, Tooltip("How dominent this animal is in the food chain, agressive animals will attack less dominant animals.")]
        private int dominance = 1;
        private int originalDominance = 0;

        // [SerializeField, Tooltip("How many seconds this animal can run for before it gets tired.")]
        private float stamina = 10f;

        [SerializeField, Tooltip("If true, this animal will never leave it's zone, even if it's chasing or running away from another animal.")]
        private bool constainedToWanderZone = false;

        [SerializeField, Tooltip("This animal will be peaceful towards species in this list.")]
        private string[] nonAgressiveTowards;

        private static List<Common_WanderScript> allAnimals = new List<Common_WanderScript>();

        public static List<Common_WanderScript> AllAnimals
        {
            get { return allAnimals; }
        }

        //[Space(), Space(5)]
        [SerializeField, Tooltip("If true, this animal will rotate to match the terrain. Ensure you have set the layer of the terrain as 'Terrain'.")]
        private bool matchSurfaceRotation = false;

        [SerializeField, Tooltip("How fast the animnal rotates to match the surface rotation.")]
        private float surfaceRotationSpeed = 2f;

        //[Space(), Space(5)]
        [SerializeField, Tooltip("If true, AI changes to this animal will be logged in the console.")]
        private bool logChanges = false;

        [SerializeField, Tooltip("If true, gizmos will be drawn in the editor.")]
        private bool showGizmos = false;

        [SerializeField] private bool drawWanderRange = true;

        public UnityEngine.Events.UnityEvent deathEvent;
        public UnityEngine.Events.UnityEvent attackingEvent;
        public UnityEngine.Events.UnityEvent idleEvent;
        public UnityEngine.Events.UnityEvent movementEvent;


        private Color distanceColor = new Color(0f, 0f, 205f);
        private Color awarnessColor = new Color(1f, 0f, 1f, 1f);
        private Color scentColor = new Color(1f, 0f, 0f, 1f);
        private Animator animator;
        private CharacterController characterController;
        private NavMeshAgent navMeshAgent;
        private Vector3 origin;

        private int totalIdleStateWeight;

        private bool useNavMesh = false;
        private Vector3 targetLocation = Vector3.zero;

        private float turnSpeed = 0f;

        public enum WanderState
        {
            Idle,
            Wander,
        }
        float MinimumStaminaForAggression
        {
            get { return stats.stamina * .9f; }
        }

        float MinimumStaminaForFlee
        {
            get { return stats.stamina * .1f; }
        }

        public WanderState CurrentState;
        float moveSpeed = 0f;
        bool forceUpdate = false;
        float idleStateDuration;
        Vector3 startPosition;
        Vector3 wanderTarget;
        IdleState currentIdleState;
        float idleUpdateTime;
        

        public void OnDrawGizmosSelected()
        {
            if (!showGizmos)
                return;

            if (drawWanderRange)
            {
                // Draw circle of radius wander zone
                Gizmos.color = distanceColor;
                Gizmos.DrawWireSphere(origin == Vector3.zero ? transform.position : origin, wanderZone);

                Vector3 IconWander = new Vector3(transform.position.x, transform.position.y + wanderZone, transform.position.z);
                Gizmos.DrawIcon(IconWander, "ico-wander", true);
            }

            if (!Application.isPlaying)
                return;

            // Draw target position.
            if (useNavMesh)
            {
                if (navMeshAgent.remainingDistance > 1f)
                {
                    Gizmos.DrawSphere(navMeshAgent.destination + new Vector3(0f, 0.1f, 0f), 0.2f);
                    Gizmos.DrawLine(transform.position, navMeshAgent.destination);
                }
            }
            else
            {
                if (targetLocation != Vector3.zero)
                {
                    Gizmos.DrawSphere(targetLocation + new Vector3(0f, 0.1f, 0f), 0.2f);
                    Gizmos.DrawLine(transform.position, targetLocation);
                }
            }
        }

        private void Awake()
        {
            if (!stats)
            {
                Debug.LogError(string.Format("No stats attached to {0}'s Wander Script.", gameObject.name));
                enabled = false;
                return;
            }

            animator = GetComponent<Animator>();

            var runtimeController = animator.runtimeAnimatorController;
            if (animator)
                animatorParameters.UnionWith(animator.parameters.Select(p=>p.name));
            
            if (logChanges)
            {
                if (runtimeController == null)
                {
                    Debug.LogError(string.Format(
                        "{0} has no animator controller, make sure you put one in to allow the character to walk. See documentation for more details (1)",
                        gameObject.name));
                    enabled = false;
                    return;
                }

                if (animator.avatar == null)
                {
                    Debug.LogError(string.Format("{0} has no avatar, make sure you put one in to allow the character to animate. See documentation for more details (2)",
                        gameObject.name));
                    enabled = false;
                    return;
                }

                if (animator.hasRootMotion == true)
                {
                    Debug.LogError(string.Format(
                        "{0} has root motion applied, consider turning this off as our script will deactivate this on play as we do not use it (3)", gameObject.name));
                    animator.applyRootMotion = false;
                }

                if (idleStates.Length == 0 || movementStates.Length == 0)
                {
                    Debug.LogError(string.Format("{0} has no idle or movement states, make sure you fill these out. See documentation for more details (4)",
                        gameObject.name));
                    enabled = false;
                    return;
                }

                if (idleStates.Length > 0)
                {
                    for (int i = 0; i < idleStates.Length; i++)
                    {
                        if (idleStates[i].animationBool == "")
                        {
                            Debug.LogError(string.Format(
                                "{0} has " + idleStates.Length +
                                " Idle states, you need to make sure that each state has an animation boolean. See documentation for more details (4)", gameObject.name));
                            enabled = false;
                            return;
                        }
                    }
                }

                if (movementStates.Length > 0)
                {
                    for (int i = 0; i < movementStates.Length; i++)
                    {
                        if (movementStates[i].animationBool == "")
                        {
                            Debug.LogError(string.Format(
                                "{0} has " + movementStates.Length +
                                " Movement states, you need to make sure that each state has an animation boolean to see the character walk. See documentation for more details (4)",
                                gameObject.name));
                            enabled = false;
                            return;
                        }

                        if (movementStates[i].moveSpeed <= 0)
                        {
                            Debug.LogError(string.Format(
                                "{0} has a movement state with a speed of 0 or less, you need to set the speed higher than 0 to see the character move. See documentation for more details (4)",
                                gameObject.name));
                            enabled = false;
                            return;
                        }

                        if (movementStates[i].turnSpeed <= 0)
                        {
                            Debug.LogError(string.Format(
                                "{0} has a turn speed state with a speed of 0 or less, you need to set the speed higher than 0 to see the character turn. See documentation for more details (4)",
                                gameObject.name));
                            enabled = false;
                            return;
                        }
                    }
                }

                if (attackingStates.Length == 0)
                {
                    Debug.Log(string.Format("{0} has " + attackingStates.Length + " this character will not be able to attack. See documentation for more details (4)",
                        gameObject.name));
                }

                if (attackingStates.Length > 0)
                {
                    for (int i = 0; i < attackingStates.Length; i++)
                    {
                        if (attackingStates[i].animationBool == "")
                        {
                            Debug.LogError(string.Format(
                                "{0} has " + attackingStates.Length +
                                " attacking states, you need to make sure that each state has an animation boolean. See documentation for more details (4)",
                                gameObject.name));
                            enabled = false;
                            return;
                        }
                    }
                }

                if (stats == null)
                {
                    Debug.LogError(string.Format("{0} has no AI stats, make sure you assign one to the wander script. See documentation for more details (5)",
                        gameObject.name));
                    enabled = false;
                    return;
                }

                if (animator)
                {
                    foreach (var item in AllStates)
                    {
                        if (!animatorParameters.Contains(item.animationBool))
                        {
                            Debug.LogError(string.Format(
                                "{0} did not contain {1}. Make sure you set it in the Animation States on the character, and have a matching parameter in the Animator Controller assigned.",
                                gameObject.name, item.animationBool));
                            enabled = false;
                            return;
                        }
                    }
                }
            }

            foreach (IdleState state in idleStates)
            {
                totalIdleStateWeight += state.stateWeight;
            }

            origin = transform.position;
            animator.applyRootMotion = false;
            characterController = GetComponent<CharacterController>();
            navMeshAgent = GetComponent<NavMeshAgent>();

            //Assign the stats to variables
            originalDominance = stats.dominance;
            dominance = originalDominance;

            stamina = stats.stamina;

            if (navMeshAgent)
            {
                useNavMesh = true;
                navMeshAgent.stoppingDistance = contingencyDistance;
            }

            if (matchSurfaceRotation && transform.childCount > 0)
            {
                transform.GetChild(0).gameObject.AddComponent<Common_SurfaceRotation>().SetRotationSpeed(surfaceRotationSpeed);
            }
        }

        IEnumerable<AIState> AllStates
        {
            get
            {
                foreach (var item in idleStates)
                    yield return item;
                foreach (var item in movementStates)
                    yield return item;
                foreach (var item in attackingStates)
                    yield return item;
                foreach (var item in deathStates)
                    yield return item;
            }
        }

        void OnEnable()
        {
            allAnimals.Add(this);
        }

        void OnDisable()
        {
            allAnimals.Remove(this);
            StopAllCoroutines();
        }


        private void Start()
        {
            startPosition = new Vector3(-7.3f, 0.66f, -5f);

            StartCoroutine(RandomStartingDelay());
        }

        bool started = false;
        readonly HashSet<string> animatorParameters = new HashSet<string>();

        void Update()
        {
            if (!started)
                return;
            if (forceUpdate)
            {
                forceUpdate = false;
            }

            var position = transform.position;
            var targetPosition = position;
            switch (CurrentState)
            {
                case WanderState.Wander:
                    stamina = Mathf.MoveTowards(stamina, stats.stamina, Time.deltaTime);
                    targetPosition = wanderTarget;
                    Debug.DrawLine(position,targetPosition,Color.yellow);
                    FaceDirection((targetPosition-position).normalized);
                    var displacementFromTarget = Vector3.ProjectOnPlane(targetPosition - transform.position, Vector3.up);
                    if (displacementFromTarget.magnitude < contingencyDistance)
                    {
                        SetState(WanderState.Idle);
                    }

                    break;
                case WanderState.Idle:
                    stamina = Mathf.MoveTowards(stamina, stats.stamina, Time.deltaTime);
                    if (Time.time>=idleUpdateTime)
                    {
                        SetState(WanderState.Wander);
                    }
                    break;
            }

            if (navMeshAgent)
            {
                navMeshAgent.destination = targetPosition;
                navMeshAgent.speed = moveSpeed;
                navMeshAgent.angularSpeed = turnSpeed;
            }
            else
                characterController.SimpleMove(moveSpeed * UnityEngine.Vector3.ProjectOnPlane(targetPosition - position,Vector3.up).normalized);


        }

        void FaceDirection(Vector3 facePosition)
        {
            transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(Vector3.RotateTowards(transform.forward,
                facePosition, turnSpeed * Time.deltaTime*Mathf.Deg2Rad, 0f), Vector3.up), Vector3.up);
        }

        bool IsValidLocation(Vector3 targetPosition)
        {
            if (!constainedToWanderZone)
                return true;
            float distanceFromWander = Vector3.Distance(startPosition, targetPosition);
            bool isInWander = distanceFromWander < wanderZone;
            return isInWander;
        }

        void SetState(WanderState state)
        {
            var previousState = CurrentState;
            //if (state != previousState)
            {
                CurrentState = state;
                switch (CurrentState)
                {
                    case WanderState.Idle:
                        HandleBeginIdle();
                        break;
                    case WanderState.Wander:
                        HandleBeginWander();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }


        void ClearAnimatorBools()
        {
            foreach (var item in idleStates) 
                TrySetBool(item.animationBool, false);
            foreach (var item in movementStates) 
                TrySetBool(item.animationBool, false);
            foreach (var item in attackingStates) 
                TrySetBool(item.animationBool, false);
            foreach (var item in deathStates) 
                TrySetBool(item.animationBool, false);
        }
        void TrySetBool(string parameterName,bool value)
        {
            if (!string.IsNullOrEmpty(parameterName))
            {
                if (logChanges||animatorParameters.Contains(parameterName))
                    animator.SetBool(parameterName, value);
            }
        }

        void HandleBeginDeath()
        {
            ClearAnimatorBools();
            if (deathStates.Length > 0) 
                TrySetBool(deathStates[Random.Range(0, deathStates.Length)].animationBool, true);

            deathEvent.Invoke();
            if (navMeshAgent && navMeshAgent.isOnNavMesh)
                navMeshAgent.destination = transform.position;
            enabled = false;
        }

        void SetMoveFast()
        {
            MovementState moveState = null;
            var maxSpeed = 0f;
            foreach (var state in movementStates)
            {
                var stateSpeed = state.moveSpeed;
                if (stateSpeed > maxSpeed)
                {
                    moveState = state;
                    maxSpeed = stateSpeed;
                }
            }

            UnityEngine.Assertions.Assert.IsNotNull(moveState, string.Format("{0}'s wander script does not have any movement states.", gameObject.name));
            turnSpeed = moveState.turnSpeed;
            moveSpeed = maxSpeed;
            ClearAnimatorBools();
            TrySetBool(moveState.animationBool,true);
        }

        void SetMoveSlow()
        {
            MovementState moveState = null;
            var minSpeed = float.MaxValue;
            foreach (var state in movementStates)
            {
                var stateSpeed = state.moveSpeed;
                if (stateSpeed < minSpeed)
                {
                    moveState = state;
                    minSpeed = stateSpeed;
                }
            }

            UnityEngine.Assertions.Assert.IsNotNull(moveState, string.Format("{0}'s wander script does not have any movement states.", gameObject.name));
            turnSpeed = moveState.turnSpeed;
            moveSpeed = minSpeed;
            ClearAnimatorBools();
            TrySetBool(moveState.animationBool, true);
        }
        void HandleBeginIdle()
        {
            var targetWeight = Random.Range(0, totalIdleStateWeight);
            var curWeight = 0;
            foreach (var idleState in idleStates)
            {
                curWeight += idleState.stateWeight;
                if (targetWeight > curWeight)
                    continue;
                idleUpdateTime = Time.time + Random.Range(idleState.minStateTime, idleState.maxStateTime);
                ClearAnimatorBools();
                TrySetBool(idleState.animationBool,true);
                moveSpeed = 0f;
                break;
            }
            idleEvent.Invoke();
        }
        void HandleBeginWander()
        {
            var rand = Random.insideUnitSphere * wanderZone;
            var targetPos = startPosition + rand;
            ValidatePosition(ref targetPos);

            wanderTarget = targetPos;
            SetMoveSlow();
        }

        void ValidatePosition(ref Vector3 targetPos)
        {
            if (navMeshAgent)
            {
                NavMeshHit hit;
                if (!NavMesh.SamplePosition(targetPos, out hit, Mathf.Infinity, 1 << NavMesh.GetAreaFromName("Walkable")))
                {
                    Debug.LogError("Unable to sample nav mesh. Please ensure there's a Nav Mesh layer with the name Walkable");
                    enabled = false;
                    return;
                }

                targetPos = hit.position;
            }
        }


        IEnumerator RandomStartingDelay()
        {
            yield return new WaitForSeconds(Random.Range(0f, 2f));
            started = true;
            StartCoroutine(ConstantTicking(Random.Range(.7f,1f)));
        }

        IEnumerator ConstantTicking(float delay)
        {
            while (true)
            {
                yield return new WaitForSeconds(delay);
            }
            // ReSharper disable once IteratorNeverReturns
        }

        [ContextMenu("This will delete any states you have set, and replace them with the default ones, you can't undo!")]
        public void BasicWanderSetUp()
        {
            MovementState walking = new MovementState(), running = new MovementState();
            IdleState idle = new IdleState();
            AIState attacking = new AIState(), death = new AIState();

            walking.stateName = "Walking";
            walking.animationBool = "isWalking";
            running.stateName = "Running";
            running.animationBool = "isRunning";
            movementStates = new MovementState[2];
            movementStates[0] = walking;
            movementStates[1] = running;


            idle.stateName = "Idle";
            idle.animationBool = "isIdling";
            idleStates = new IdleState[1];
            idleStates[0] = idle;
        }
    }
}
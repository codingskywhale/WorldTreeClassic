using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Polyperfect.Common
{
    [CreateAssetMenu(fileName = "New Wander Stats", menuName = "PolyPerfect/New Wander Stats", order = 1)]
    public class AIStats : ScriptableObject
    {
        [SerializeField, Tooltip("How dominent this is in the food chain, more agressive will attack less dominant.")]
        public int dominance = 1;

        [SerializeField, Tooltip("How many seconds this can run for before it gets tired.")]
        public float stamina = 10f;

        [SerializeField, Tooltip("How much health this has.")]
        public float toughness = 5f;
    }
}

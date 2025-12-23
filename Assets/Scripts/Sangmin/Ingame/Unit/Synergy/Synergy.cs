using System;
using UnityEngine;

namespace Sangmin
{
    public class Synergy : ScriptableObject, ISynergy
    {
        public enum SynergyName
        {
            WARRIOR = 0,
            RANGER = 1,
            SORCERER = 2,
            SUPPORTER = 3
        }

        public SynergyName synergyName;

        public int count { get; set;}

        public void OnAttack(Unit self)
        {
            throw new System.NotImplementedException();
        }

        public void OnChangeOnce(Unit self)
        {
            throw new System.NotImplementedException();
        }

        public void OnCombatStart(Unit self)
        {
            throw new System.NotImplementedException();
        }

        public void OnCooldownUp(Unit self)
        {
            throw new System.NotImplementedException();
        }

        public void OnStack(Unit self)
        {
            throw new System.NotImplementedException();
        }

        public void OnStackFull(Unit self)
        {
            throw new System.NotImplementedException();
        }
    }
}
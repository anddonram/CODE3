using Codigo.Behaviors;
using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Framework;

namespace Codigo.Components
{
    class PersonFightTraits : FightTraits
    {
        [RequiredComponent]
        Person person;

        public override Boolean CanAttack()
        {
            return true;
        }
        public override float GetAttack()
        {
            return person.GetAttack();
        }
        public override void AfterAttack()
        {
            person.GainFightPoints();
        }
        public override float GetAttackSpeed()
        {
            return person.GetAttackSpeed();
        }
    }
}
﻿namespace IrisFenrir.AI.FSM
{
    public class AndCondition<T> : FSMCondition<T>
    {
        private FSMCondition<T> m_condition1;
        private FSMCondition<T> m_condition2;

        public AndCondition(FSMCondition<T> con1, FSMCondition<T> con2)
        {
            m_condition1 = con1;
            m_condition2 = con2;
        }

        public override bool Condition(T owner)
        {
            return m_condition1.Condition(owner) && m_condition2.Condition(owner);
        }
    }
}

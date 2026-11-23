// AI를 활용한 배운 문법을 작성하는 페이지 입니다.

// 클래스. 생성자.
// 클래스로 무엇을 할래? 

// 'AI 읽을 수 있는 범위' 만큼만 만들어달라.
// 클래스 한개만 만들어라.
// 내가 어떻게 만들건지 문법을 설명할거다.

// AI. 정해지지 않은 내용은 자기가 알아서 만든다. 다음 똑같이 프롬프트를 입력해도 결과가 다르게 나온다.
// 클래스를 직접 설계 하신다음에. 그 클래스로 while반복문 입력, 업데이트, 렌더링 으로 만들어줘

using System;

namespace TextRpgGame
{
    public class Unit
    {
        public string Name { get; set; }
        public int MaxHp { get; private set; }
        public int CurrentHp { get; private set; }
        public int AttackPower { get; set; }
        public bool IsDead => CurrentHp <= 0;

        public Unit(string name, int maxHp, int attackPower)
        {
            Name = name;
            MaxHp = maxHp;
            CurrentHp = maxHp;
            AttackPower = attackPower;
        }

        public void Attack(Unit target)
        {
            if (IsDead) return;
            target.TakeDamage(AttackPower);
        }

        public void TakeDamage(int damage)
        {
            CurrentHp -= damage;
            if (CurrentHp < 0) CurrentHp = 0;
        }

        public void Heal(int amount)
        {
            if (IsDead) return;
            CurrentHp += amount;
            if (CurrentHp > MaxHp) CurrentHp = MaxHp;
        }
    }
}
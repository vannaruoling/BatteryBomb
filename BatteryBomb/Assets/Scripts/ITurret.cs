public interface ITurret
{
    bool IsPowered { get; }
    bool IsDead { get; }
    void SetPowered(bool powered);
    void Die();
    void Revive();
}
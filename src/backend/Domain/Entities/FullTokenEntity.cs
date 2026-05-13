namespace Domain.Entities
{
    public class FullTokenEntity : BaseEntity
    {
        public override string Id { get => Value; set => Value = value; }

        public string Value { get; set; } = default!;

        public string UserLogin { get; set; } = default!;

        public DateTime ExpiresAt { get; set; }


    }
}

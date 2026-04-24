namespace Telemachus.Business.Models.Cargo
{
    public class CargoDTO
    {
        public int? CargoId { get; set; }
        public int? CargoDetailId { get; set; }
        public string UserId { get; set; }
        public int? GradeId { get; set; }
        public int? Parcel { get; set; }
        public int? Quantity { get; set; }
    }
}

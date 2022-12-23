namespace SFA.DAS.Roatp.Domain.Models;

public class DeliveryModelWithAddress : DeliveryModel
{
    public string Address1 { get; set; }
    public string Address2 { get; set; }
    public string Town { get; set; }
    public string County { get; set; }
    public string Postcode { get; set; }
}

namespace Api.Data.Dtos
{
    /// <summary>
    /// DTO for the next available MAC address response
    /// </summary>
    public class NextMacAddressDto
    {
        /// <summary>
        /// ID of the MAC address
        /// </summary>
        public int MacId { get; set; }

        /// <summary>
        /// MAC address value (format: 00:1A:2B:00:00:01)
        /// </summary>
        public string MacAddress { get; set; } = string.Empty;
    }
}

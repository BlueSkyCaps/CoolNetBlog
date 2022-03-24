using CommonObject.Enums;

namespace CommonObject.Constructs
{
    public class ValueResult
    {
        public ValueCodes Code { get; set; }
        public string? TipMessage { get; set; } = "";
        public string? HideMessage { get; set; } = "";
        public dynamic DmData { get; set; }
        public object Data { get; set; }
    }
}

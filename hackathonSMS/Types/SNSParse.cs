namespace Hackathon.Types
{
    public class SNSParse
    {
        public object originationNumber {get; set; }
        public object destinationNumber { get; set; }
        public object messageKeyword { get; set; }
        public object messageBody { get; set; }
        public object inboundMessageId { get; set; }
        public object previousPublishedMessageId { get; set; }
    }

}
class QCSFixrAttendeeData : FixrAttendeeData
{
    public string Course { get; set; } = "";
    public string StudentNo { get; set; } = "";
    public string Year { get; set; } = "";

    public List<Object> asSUList()
    {
        return new List<object>() {
           "Queen's Computing Society",
           FirstName,
           LastName,
           (TicketTypeName=="Non-Student Membership")?"Not Provided":StudentNo,
           (TicketTypeName=="Non-Student Membership")?"Not Provided":Email,
           (TicketTypeName=="Non-Student Membership")?Email:"Not Provided",

       };
    }
    public List<Object> asFIXRReducedDetailList()
    {
        return new List<object>() {
           FirstName,
           LastName,
           (TicketTypeName=="Non-Student Membership")?"Not Provided":StudentNo,
           (TicketTypeName=="Non-Student Membership")?"Not Provided":Email,
           (TicketTypeName=="Non-Student Membership")?Email:"Not Provided",

       };
    }

      public List<Object> asFIXRList()
    {
        return new List<object>() {
            SoldAt,
           FirstName,
           LastName,
          Email,
          Course,
          
    
           (TicketTypeName=="Non-Student Membership")?"Not Provided":StudentNo,
           Year,
           TicketTypeName

       };
    }
}
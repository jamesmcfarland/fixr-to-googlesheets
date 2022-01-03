class QCSFixrAttendeeData : FixrAttendeeData
{
    public string Course { get; set; } = "";
    public string StudentNo { get; set; } = "";
    private string _QUBEmail = "";
    public string QUBEmail
    {
        get
        {
            if (_QUBEmail == "" || !_QUBEmail.Contains("qub.ac.uk"))
            {
                if (!Email.Contains("qub.ac.uk"))
                {
                    //Use Active Directory email
                    return $"{StudentNo}@ads.qub.ac.uk";

                }
                else return Email;
            }
            else return _QUBEmail;
        }
        set
        {
            _QUBEmail = value;
        }
    }
    public string Year { get; set; } = "";

    public List<Object> asSUList()
    {
        return new List<object>() {
           "Queen's Computing Society",
           FirstName,
           LastName,
           (TicketTypeName=="Non-Student Membership")?"Not Provided":StudentNo,
           (TicketTypeName=="Non-Student Membership")?"Not Provided":QUBEmail,
           (TicketTypeName=="Non-Student Membership")?Email:"Not Provided",

       };
    }
    public List<Object> asFIXRReducedDetailList()
    {
        return new List<object>() {
           FirstName,
           LastName,
           (TicketTypeName=="Non-Student Membership")?"Not Provided":StudentNo,
           (TicketTypeName=="Non-Student Membership")?"Not Provided":QUBEmail,
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
            TicketTypeName,
            QUBEmail

       };
    }
}
using Microsoft.AspNetCore.Html;
using Student.Models;
namespace HTML;

public static class HTMLHelper {
    // private readonly MeetingDto? _data;
    // private readonly CandidateModel _candidate;

    // public HTMLHelper(MeetingDto data) {
    //     _data = data;
    // }

    // public HTMLHelper(CandidateModel candidate)
    // {
    //     _candidate = candidate;
    // }

    public static string Rejection(string firstname) {
        return $@"<html>
<body style='color: black; font-size: 14px; font-weight: normal; padding: 10px'>
    <div>
        <p style='color: black; font-size: 14px; font-weight: normal;'>
            Dear {firstname},
        </p>
    </div>
    <div>
        <p style='color: black; font-size: 14px; font-weight: normal;'>
            Thank you for giving us the opportunity to consider you for employment. We have reviewed your profile but regret that at this time we will not be moving forward with your application. 
        </p>
    </div>

    <div>
        <p style='color: black; font-size: 14px; font-weight: normal;'>
            With your approval, we will be keeping your resume for future job openings when they arise.
        </p>
    </div>
    <div>
        <p style='color: black; font-size: 14px; font-weight: normal;'>
        We wish you every success with your job search and thank you for your interest in our bank. 
        </p>
    </div>
    <div>
    <p style='color: black; font-size: 14px; font-weight: normal;'>
    Sincerely,
    </p>
    </div>
    <p style='color: black; font-size: 14px; font-weight: normal;'>
    The Talent Acquisition Team
    </p>
    <p style='color: black; font-size: 14px; font-weight: normal;'>
    LAPO Microfinance Bank
    </p>
</body>
<footer style='color: black; font-size: 14px; font-weight: normal; padding: 10px;'>
    <div style='color: black; font-size: 14px; font-weight: normal;'><p>LAPO MfB Careers Team  </p></div>
    <div style='flex:auto; flex-direction: row;'>
        <p style='color: orange; font-weight: 400;'>Head Office:</p>
        <p style='color: black; font-size: 14px; font-weight: normal;'>LAPO Development Centre, 15 Ikorodu Road, Maryland Bus Stop, Lagos, Nigeria.</p>
    </div>
    <div style='flex:auto; flex-direction: row;'>
        <p style='color: orange; font-weight: 400;'>Annex:</p>
        <p style='color: black; font-size: 14px; font-weight: normal;'>LAPO Place, 18 Dawson Road, P.M.B 1729, Benin City, Edo State. Nigeria.</p>
    </div>
    <div style='color: black; font-size: 14px; font-weight: normal;'>
        +2348063803863 
    </div>
    <div style='color: black; font-size: 11px; font-weight: normal;'>
        paul.obasuyi@lapo-nigeria.org 
        www.lapo-nigeria.org   
    </div>
</footer>
</html>";
}

public static string PasswordReset(string firstname, string id) {
    return $@"<!DOCTYPE html>
<html>
<body style='padding: 10px; color: black; font-size: 14px; font-weight: normal;'>
    <div>
        <p style='color: black; font-size: 14px; font-weight: normal;'>
            Dear {firstname}
        </p>
        <div style='color: black; font-size: 14px; font-weight: normal;'>
            To reset your password please click <a href='https://career.lapo-nigeria.org/password_reset?id={id}&token=00727143910'>here</a>
        </div>
    </div>
</body>
<footer style='color: black; font-size: 11px; font-weight: normal;'>
    <div style='color: black; font-size: 11px; font-weight: normal;'><p>LAPO MfB Careers Team  </p></div>
    <div style='flex:auto; flex-direction: row; color: black; font-size: 11px; font-weight: normal;'>
        <p style='color: orange; font-weight: 400;'>Head Office:</p>
        <p>LAPO Development Centre, 15 Ikorodu Road, Maryland Bus Stop, Lagos, Nigeria.</p>
    </div>
    <div style='flex:auto; flex-direction: row; color: black; font-size: 11px; font-weight: normal;'>
        <p style='color: orange; color: black; font-size: 11px; font-weight: normal;'>Annex:</p>
        <p style='color: black; font-size: 11px; font-weight: normal;'>LAPO Place, 18 Dawson Road, P.M.B 1729, Benin City, Edo State. Nigeria.</p>
    </div>
    <div>
        +2348063803863 
    </div>
    <div style='color: black; font-size: 11px; font-weight: normal;'>
        paul.obasuyi@lapo-nigeria.org 
        www.lapo-nigeria.org   
    </div>
</footer>
</html>";
}

public static string VerifyEmail(string firstname, string id) {
    return $@"<!DOCTYPE html>
<html>
<body style='padding: 10px; color: black; font-size: 14px; font-weight: normal;'>
    <div>
        <p style='color: black; font-size: 14px; font-weight: normal;'>
            Hi {firstname},
        </p>
    </div>
    <div>
        <p style='color: black; font-size: 14px; font-weight: normal;'>
            Please Confirm your email address by clicking <a href='https://localhost:3000/emailConfirmation?id={id}'>here</a>
        </p>
    </div>
    <div style='color: black; font-size: 14px; font-weight: normal;'>
        We look forward to meeting you!
    </div>
    <div style='color: black; font-size: 14px; font-weight: normal; margin-top: 20px;'>
        Kind regards,
    </div>
</body>
<footer style='color: black; font-size: 11px; font-weight: normal;'>
    <div style='color: black; font-size: 11px; font-weight: normal;'><p>LAPO MfB Careers Team  </p></div>
    <div style='flex:auto; flex-direction: row; color: black; font-size: 11px; font-weight: normal;'>
        <p style='color: orange; font-weight: 400;'>Head Office:</p>
        <p>LAPO Development Centre, 15 Ikorodu Road, Maryland Bus Stop, Lagos, Nigeria.</p>
    </div>
    <div style='flex:auto; flex-direction: row; color: black; font-size: 11px; font-weight: normal;'>
        <p style='color: orange; color: black; font-size: 11px; font-weight: normal;'>Annex:</p>
        <p style='color: black; font-size: 11px; font-weight: normal;'>LAPO Place, 18 Dawson Road, P.M.B 1729, Benin City, Edo State. Nigeria.</p>
    </div>
    <div>
        +2348063803863 
    </div>
    <div style='color: black; font-size: 11px; font-weight: normal;'>
        paul.obasuyi@lapo-nigeria.org 
        www.lapo-nigeria.org   
    </div>
</footer>
</html>";
}

public static string SendId(string firstname, string id) {
    return $@"<!DOCTYPE html>
<html>
<body style='padding: 10px; color: black; font-size: 14px; font-weight: normal;'>
    <div>
        <p style='color: black; font-size: 14px; font-weight: normal;'>
            Hi {firstname},
        </p>
    </div>
    <div>
        <p style='color: black; font-size: 14px; font-weight: normal;'>
            Please use {id} as your unique Id and register on the platform by clicking <a href='https://localhost:3000/student'>here</a>
        </p>
    </div>
    <div style='color: black; font-size: 14px; font-weight: normal;'>
        We look forward to meeting you!
    </div>
    <div style='color: black; font-size: 14px; font-weight: normal; margin-top: 20px;'>
        Kind regards,
    </div>
</body>
<footer style='color: black; font-size: 11px; font-weight: normal;'>
    <div style='color: black; font-size: 11px; font-weight: normal;'><p>LAPO MfB Careers Team  </p></div>
    <div style='flex:auto; flex-direction: row; color: black; font-size: 11px; font-weight: normal;'>
        <p style='color: orange; font-weight: 400;'>Head Office:</p>
        <p>LAPO Development Centre, 15 Ikorodu Road, Maryland Bus Stop, Lagos, Nigeria.</p>
    </div>
    <div style='flex:auto; flex-direction: row; color: black; font-size: 11px; font-weight: normal;'>
        <p style='color: orange; color: black; font-size: 11px; font-weight: normal;'>Annex:</p>
        <p style='color: black; font-size: 11px; font-weight: normal;'>LAPO Place, 18 Dawson Road, P.M.B 1729, Benin City, Edo State. Nigeria.</p>
    </div>
    <div>
        +2348063803863 
    </div>
    <div style='color: black; font-size: 11px; font-weight: normal;'>
        paul.obasuyi@lapo-nigeria.org 
        www.lapo-nigeria.org   
    </div>
</footer>
</html>";
}

public static string Reject(string firstname, string reason) {
    return $@"<!DOCTYPE html>
<html>
<body style='padding: 10px; color: black; font-size: 14px; font-weight: normal;'>
    <div>
        <p style='color: black; font-size: 14px; font-weight: normal;'>
            Hi {firstname},
        </p>
    </div>
    <div>
        <p style='color: black; font-size: 14px; font-weight: normal;'>
            Unfortunately we won't be processing your scholarship application anymore because {reason}
        </p>
    </div>
    <div style='color: black; font-size: 14px; font-weight: normal;'>
        We look forward to meeting you!
    </div>
    <div style='color: black; font-size: 14px; font-weight: normal; margin-top: 20px;'>
        Kind regards,
    </div>
</body>
<footer style='color: black; font-size: 11px; font-weight: normal;'>
    <div style='color: black; font-size: 11px; font-weight: normal;'><p>LAPO MfB Careers Team  </p></div>
    <div style='flex:auto; flex-direction: row; color: black; font-size: 11px; font-weight: normal;'>
        <p style='color: orange; font-weight: 400;'>Head Office:</p>
        <p>LAPO Development Centre, 15 Ikorodu Road, Maryland Bus Stop, Lagos, Nigeria.</p>
    </div>
    <div style='flex:auto; flex-direction: row; color: black; font-size: 11px; font-weight: normal;'>
        <p style='color: orange; color: black; font-size: 11px; font-weight: normal;'>Annex:</p>
        <p style='color: black; font-size: 11px; font-weight: normal;'>LAPO Place, 18 Dawson Road, P.M.B 1729, Benin City, Edo State. Nigeria.</p>
    </div>
    <div>
        +2348063803863 
    </div>
    <div style='color: black; font-size: 11px; font-weight: normal;'>
        paul.obasuyi@lapo-nigeria.org 
        www.lapo-nigeria.org   
    </div>
</footer>
</html>";
}
public static string PushBack(string firstname, string reason) {
    return $@"<!DOCTYPE html>
<html>
<body style='padding: 10px; color: black; font-size: 14px; font-weight: normal;'>
    <div>
        <p style='color: black; font-size: 14px; font-weight: normal;'>
            Hi {firstname},
        </p>
    </div>
    <div>
        <p style='color: black; font-size: 14px; font-weight: normal;'>
            {reason}
        </p>
    </div>
    <div style='color: black; font-size: 14px; font-weight: normal;'>
        We look forward to meeting you!
    </div>
    <div style='color: black; font-size: 14px; font-weight: normal; margin-top: 20px;'>
        Kind regards,
    </div>
</body>
<footer style='color: black; font-size: 11px; font-weight: normal;'>
    <div style='color: black; font-size: 11px; font-weight: normal;'><p>LAPO MfB Careers Team  </p></div>
    <div style='flex:auto; flex-direction: row; color: black; font-size: 11px; font-weight: normal;'>
        <p style='color: orange; font-weight: 400;'>Head Office:</p>
        <p>LAPO Development Centre, 15 Ikorodu Road, Maryland Bus Stop, Lagos, Nigeria.</p>
    </div>
    <div style='flex:auto; flex-direction: row; color: black; font-size: 11px; font-weight: normal;'>
        <p style='color: orange; color: black; font-size: 11px; font-weight: normal;'>Annex:</p>
        <p style='color: black; font-size: 11px; font-weight: normal;'>LAPO Place, 18 Dawson Road, P.M.B 1729, Benin City, Edo State. Nigeria.</p>
    </div>
    <div>
        +2348063803863 
    </div>
    <div style='color: black; font-size: 11px; font-weight: normal;'>
        paul.obasuyi@lapo-nigeria.org 
        www.lapo-nigeria.org   
    </div>
</footer>
</html>";
}
}
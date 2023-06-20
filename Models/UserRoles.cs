using System.ComponentModel;

namespace MicroServices
{
    public enum UserRoleName
    {
        [Description("Student")]
        Student = 1,

        [Description("Control of back-office platform technical setup")]
        SystemAdministrator = 2,

        [Description("Trainer")]
        Trainer = 3,

        [Description("Read only access to all areas")]
        AllAccessViewer = 4
    }
}
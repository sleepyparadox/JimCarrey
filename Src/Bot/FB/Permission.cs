
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JimCarrey.FB
{
    public class Permission
    {
        public string permission { get; set; }
        public string status { get; set; }

        public PermissionStatus Status
        {
            get
            {
                switch(status)
                {
                    case "granted":
                        return PermissionStatus.Granted;
                    case "declined":
                        return PermissionStatus.Decliend;
                    default:
                        return PermissionStatus.Default;
                }
            }
        }
    }

    public enum PermissionStatus
    {
        Default,
        Granted,
        Decliend
    }
}

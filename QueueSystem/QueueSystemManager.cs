using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueSystem
{
    public class QueueSystemManager
    {
        QueueCache queueCache;
        MemberCache memberCache;

        public QueueSystemManager()
        {
            queueCache = new QueueCache();
            memberCache = new MemberCache();
        }

        public QueueCache QueueCache
        {
            get
            {
                return queueCache;
            }
        }

        public MemberCache MemberCache
        {
            get
            {
                return memberCache;
            }
        }




    }
}

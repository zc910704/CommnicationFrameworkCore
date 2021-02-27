using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommDeviceCore
{
    public interface IPhyCommDevice: IObservable<ILayPackageSendResult>
    {
        public bool Open();

        public bool Close();

        public Task<ILayPackageSendResult> Send(ILayPackage package);
    }
}

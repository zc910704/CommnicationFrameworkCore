using System;
using System.Collections.Generic;
using System.Text;

namespace CommDeviceCore.Protocol
{
    public interface IProtocol<TIn, TOut>
        where TIn: ILayerPackage
        where TOut : ILayerPackage
    {
        public TOut Pack(TIn layerPackage);

        public TIn UnPack(TOut layerackage);
    }
}

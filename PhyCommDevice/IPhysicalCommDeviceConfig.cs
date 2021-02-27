using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CommDeviceCore.PhyCommDevice
{
    public interface IPhysicalCommDeviceConfig
    {
        public DeviceConfigCollection DeviceConfigs { get; set; }
    }

public class DeviceConfigCollection : ICollection<KeyValuePair<string, string>>
{
    int ICollection<KeyValuePair<string, string>>.Count => throw new NotImplementedException();

    bool ICollection<KeyValuePair<string, string>>.IsReadOnly => throw new NotImplementedException();

    void ICollection<KeyValuePair<string, string>>.Add(KeyValuePair<string, string> item)
    {
        throw new NotImplementedException();
    }

    void ICollection<KeyValuePair<string, string>>.Clear()
    {
        throw new NotImplementedException();
    }

    bool ICollection<KeyValuePair<string, string>>.Contains(KeyValuePair<string, string> item)
    {
        throw new NotImplementedException();
    }

    void ICollection<KeyValuePair<string, string>>.CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }

    IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }

    bool ICollection<KeyValuePair<string, string>>.Remove(KeyValuePair<string, string> item)
    {
        throw new NotImplementedException();
    }
}
}

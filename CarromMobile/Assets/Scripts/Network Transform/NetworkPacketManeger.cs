using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class NetworkPacketManeger<T> where T : class
{
    public event System.Action<byte[]> onRequirePackageTransmit;      //when we wanna transmit packet this event will be raised

    private float m_SendSpeed = .2f;
    public float sendSpeed
    {
        get
        {
            if (m_SendSpeed < 0.1f)
                return m_SendSpeed = .1f;
            return m_SendSpeed;
        }
        set
        {
            m_SendSpeed = value;
        }
    }

    float nextTick;

    private List<T> m_Packages;     //whatever is in here we gonna meake a list out of it
    public List<T> Packages
    {
        get
        {
            if (m_Packages == null)
                m_Packages = new List<T>();
            return m_Packages;
        }
    }

    public Queue<T> receivePackages;        //if we take one it will automaticly get removes from the queue

    /// <summary>
    /// Add new Packages to the List T we Would like to transmit soon.
    /// </summary>
    /// <param name="package"></param>
    public void AddPackages(T package)
    {
        Packages.Add(package);
    }

    public void ClearQueue()
    {
        if (receivePackages != null)
            receivePackages.Clear();

    }

    /// <summary>
    /// Process any data received
    /// </summary>
    /// <param name="bytes"></param>
    public void ReceiveData(byte[] bytes)
    {
        if (receivePackages == null)
            receivePackages = new Queue<T>();

        T[] packages = ReadBytes(bytes).ToArray();           //read bytes into an array
        for (int i = 0; i < packages.Length; i++)
        {
            receivePackages.Enqueue(packages[i]);

        }


    }
    public void Tick()
    {
        nextTick += 1 / this.sendSpeed * Time.fixedDeltaTime;
        if (nextTick > 1 && Packages.Count > 0)                      //start transmitting if nextTick>1 and if there is any data in packages
        {
            nextTick = 0;

            if (onRequirePackageTransmit != null)
            {
                byte[] bytes = CreateBytes();              //creat an array called bytes out of data in packages list
                Packages.Clear();                          //the clear packages
                onRequirePackageTransmit(bytes);
            }
        }
    }

    public T GetNextDataReceived()
    {
        if (receivePackages == null || receivePackages.Count == 0)
            return default(T);

        return receivePackages.Dequeue();    
    }

    byte[] CreateBytes()
    {
      /*  for (int i = 1; i < Packages.Count - 1; i =+ 2)
        {
            Packages.RemoveAt(i);
        }*/
        /*  if (Packages.Count > 2)
          {
              Packages.RemoveRange(1, Packages.Count - 2);
          }*/
        BinaryFormatter formatter = new BinaryFormatter();
        using (MemoryStream ms = new MemoryStream())
        {
            formatter.Serialize(ms, this.Packages);
            return ms.ToArray();
        }
    }
    List<T> ReadBytes(byte[] bytes)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        using (MemoryStream ms = new MemoryStream())
        {
            ms.Write(bytes, 0, bytes.Length);
            ms.Seek(0, SeekOrigin.Begin);
            return (List<T>)formatter.Deserialize(ms);
        }
    }
}

using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MisceTests : MonoBehaviour
{
    [Test]
    public void TestNoDoubleInList()
    {

        List<int> list = new List<int>();
        HashSet<int> set = new HashSet<int>();

        for (int i = 0; i < 1000; i++)
        {
            list.Add(i);
        }

        int nbInList = list.Count;

        for (int i = 0; i < nbInList; i++)
        {
            for (int j = i + 1; j < nbInList; j++)
            {
                var num = list[i] + list[j] * 10000000;

                Assert.False(set.Contains(num));

                set.Add(num);

            }
        }
    }

    [Test]
    public void TestColorConversion()
    {
        var systemColor = System.Drawing.Color.AliceBlue;

        var unityColor = systemColor.ToUnityColor();

        var systemColorB = unityColor.ToSystemColor();

        Assert.True(systemColorB.Equals(systemColor));

    }
}

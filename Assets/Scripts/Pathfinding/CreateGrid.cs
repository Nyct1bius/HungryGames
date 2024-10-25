using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateGrid : MonoBehaviour
{
    public GameObject GridUnit;
    public uint Width;
    public uint Height;
    public uint Depth;

    // Start is called before the first frame update
    void Start()
    {
        for (int x = 0; x < Width; ++x)
        {
            for (int y = 0; y < Height; ++y)
            {
                for (int z = 0; z < Depth; ++z)
                {
                    if (x > 0 && x < Width - 1 &&
                        y > 0 && y < Height - 1 &&
                        z > 0 && z < Depth - 1)
                        continue;

                    Instantiate(GridUnit, new Vector3(x, y, z), Quaternion.identity);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

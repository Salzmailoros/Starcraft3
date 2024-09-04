using UnityEngine;

public class PoolableObject : MonoBehaviour
{
    [SerializeField]private string Key;
    public string ReturnKey()
    {
        return Key;
    }
    public void SetKey(string newKey)
    {
        Key = newKey;
    }
}

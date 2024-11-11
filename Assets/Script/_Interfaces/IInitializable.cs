using System.Collections;

public interface IInitializable
{
    string InitializationDisplayText { get; }
    bool IsInitialized { get; }
    IEnumerator Init();
}
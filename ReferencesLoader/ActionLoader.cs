using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine;
using System;


namespace GameZone.Scripts.Loader
{
    public class ActionLoader : MonoBehaviour
{
    public Action OnStartLoad;
    public Action<string> OnInvokeAction;
    public Action OnEndLoad;
    public Action<Exception> OnErrorLoad;
    
    [SerializeField] private List<MonoBehaviour> scripts = new List<MonoBehaviour>();
    private List<ILoadAction> actions = new List<ILoadAction>();

    private bool actionsIsLoading = false;
    
    private void Awake() {
        scripts.ForEach((mono) => {
            if (mono is ILoadAction action) {
                actions.Add(action);
            }
        }); 
        actions = actions.OrderBy(action => action.Priority).ToList();
    }

    public async void LoadActions() {
        if (actionsIsLoading) return;
        actionsIsLoading = true;
        
        OnStartLoad?.Invoke();

        foreach (var action in actions) {
            OnInvokeAction?.Invoke(action.DebugText());

            try {
                Debug.Log("Action");
                await action.LoadAction();
            }
            catch (Exception e) {
                OnErrorLoad?.Invoke(e);
                Debug.Log("ERROR");
                throw;
            }
            finally {
                actionsIsLoading = false;
            }
        }
        Debug.Log("END");
        
        OnEndLoad?.Invoke();
    }
    
    public async void LoadActions(List<ILoadAction> actions) {
        if (actionsIsLoading) return;
        actionsIsLoading = true;
        
        OnStartLoad?.Invoke();

        foreach (var action in actions) {
            OnInvokeAction?.Invoke(action.DebugText());

            try {
                await action.LoadAction();
            }
            catch (Exception e) {
                OnErrorLoad?.Invoke(e);
                throw;
            }
            finally {
                actionsIsLoading = false;
            }
        }
        
        OnEndLoad?.Invoke();
        
        actionsIsLoading = false;
    }

    public List<ILoadAction> GetActions() => actions;
}

public interface ILoadAction
{
    Task LoadAction();
    string DebugText();
    int Priority { set; get; }
}

public interface ILoadActionTest
{
    Task<string> LoadAction();
    int Priority { set; get; }
}

}

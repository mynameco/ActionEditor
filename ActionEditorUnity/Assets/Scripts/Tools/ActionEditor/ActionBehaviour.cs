using System;
using ActionEditor;
using UnityEngine;

public class ActionBehaviour : MonoBehaviour
{
	private void Awake()
	{
		CreateIfNeed();
    }

	public void CreateIfNeed()
	{
		if (ActionGraph != null)
			return;

		ActionGraph = new ActionGraph();
		ActionGraph.Load(store);
	}

	public void Save()
	{
		store.Clear();
        ActionGraph.Save(store);

#if UNITY_EDITOR
		UnityEditor.EditorUtility.SetDirty(this);
#endif
	}

	[SerializeField]
	private ActionGraphStore store;

	[NonSerialized]
	public ActionGraph ActionGraph;
}

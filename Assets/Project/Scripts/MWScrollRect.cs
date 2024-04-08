using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MWScrollRect : ScrollRect
{
	private bool routeToParent;

	private void DoForParents<T>(Action<T> action) where T : IEventSystemHandler
	{
		Transform parent = base.transform.parent;
		while (parent != null)
		{
			Component[] components = parent.GetComponents<Component>();
			foreach (Component component in components)
			{
				if (component is T)
				{
					action((T)(IEventSystemHandler)component);
				}
			}
			parent = parent.parent;
		}
	}

	public override void OnInitializePotentialDrag(PointerEventData eventData)
	{
		DoForParents(delegate(IInitializePotentialDragHandler parent)
		{
			parent.OnInitializePotentialDrag(eventData);
		});
		base.OnInitializePotentialDrag(eventData);
	}

	public override void OnDrag(PointerEventData eventData)
	{
		if (routeToParent)
		{
			DoForParents(delegate(IDragHandler parent)
			{
				parent.OnDrag(eventData);
			});
		}
		else
		{
			base.OnDrag(eventData);
		}
	}

	public override void OnBeginDrag(PointerEventData eventData)
	{
		if (!base.horizontal)
		{
			Vector2 delta = eventData.delta;
			float num = Math.Abs(delta.x);
			Vector2 delta2 = eventData.delta;
			if (num > Math.Abs(delta2.y))
			{
				routeToParent = true;
				goto IL_00ad;
			}
		}
		if (!base.vertical)
		{
			Vector2 delta3 = eventData.delta;
			float num2 = Math.Abs(delta3.x);
			Vector2 delta4 = eventData.delta;
			if (num2 < Math.Abs(delta4.y))
			{
				routeToParent = true;
				goto IL_00ad;
			}
		}
		routeToParent = false;
		goto IL_00ad;
		IL_00ad:
		if (routeToParent)
		{
			DoForParents(delegate(IBeginDragHandler parent)
			{
				parent.OnBeginDrag(eventData);
			});
		}
		else
		{
			base.OnBeginDrag(eventData);
		}
	}

	public override void OnEndDrag(PointerEventData eventData)
	{
		if (routeToParent)
		{
			DoForParents(delegate(IEndDragHandler parent)
			{
				parent.OnEndDrag(eventData);
			});
		}
		else
		{
			base.OnEndDrag(eventData);
		}
		routeToParent = false;
	}
}

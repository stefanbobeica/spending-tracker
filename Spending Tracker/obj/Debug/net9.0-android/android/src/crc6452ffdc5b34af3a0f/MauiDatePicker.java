package crc6452ffdc5b34af3a0f;


public class MauiDatePicker
	extends androidx.appcompat.widget.AppCompatEditText
	implements
		mono.android.IGCUserPeer,
		android.view.View.OnClickListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_getDefaultMovementMethod:()Landroid/text/method/MovementMethod;:GetGetDefaultMovementMethodHandler\n" +
			"n_onClick:(Landroid/view/View;)V:GetOnClick_Landroid_view_View_Handler:Android.Views.View/IOnClickListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"";
		mono.android.Runtime.register ("Microsoft.Maui.Platform.MauiDatePicker, Microsoft.Maui", MauiDatePicker.class, __md_methods);
	}

	public MauiDatePicker (android.content.Context p0)
	{
		super (p0);
		if (getClass () == MauiDatePicker.class) {
			mono.android.TypeManager.Activate ("Microsoft.Maui.Platform.MauiDatePicker, Microsoft.Maui", "Android.Content.Context, Mono.Android", this, new java.lang.Object[] { p0 });
		}
	}

	public MauiDatePicker (android.content.Context p0, android.util.AttributeSet p1)
	{
		super (p0, p1);
		if (getClass () == MauiDatePicker.class) {
			mono.android.TypeManager.Activate ("Microsoft.Maui.Platform.MauiDatePicker, Microsoft.Maui", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android", this, new java.lang.Object[] { p0, p1 });
		}
	}

	public MauiDatePicker (android.content.Context p0, android.util.AttributeSet p1, int p2)
	{
		super (p0, p1, p2);
		if (getClass () == MauiDatePicker.class) {
			mono.android.TypeManager.Activate ("Microsoft.Maui.Platform.MauiDatePicker, Microsoft.Maui", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android:System.Int32, System.Private.CoreLib", this, new java.lang.Object[] { p0, p1, p2 });
		}
	}

	public android.text.method.MovementMethod getDefaultMovementMethod ()
	{
		return n_getDefaultMovementMethod ();
	}

	private native android.text.method.MovementMethod n_getDefaultMovementMethod ();

	public void onClick (android.view.View p0)
	{
		n_onClick (p0);
	}

	private native void n_onClick (android.view.View p0);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}

using System;
using System.Runtime.InteropServices;

namespace jxshell.dotnet4
{
	[ComVisible(true)]
	public class delegateWrapperStatic : wrapperStatic
	{
		public delegateWrapperStatic(Type t, typeDescriptor td) : base(t, td)
		{
		}

		public object construct(object target, string method)
		{
			delegateWrapper thisWrapper = this.getThisWrapper();
			Delegate wrappedObject = Delegate.CreateDelegate(this.wrappedType, thisWrapper, "__internalInvoke");
			thisWrapper.wrappedObject = wrappedObject;
			thisWrapper.wrappedType = this.wrappedType;
			thisWrapper.typeD = this.typeD;
			thisWrapper.__internalMethod = method;
			thisWrapper.__internalTarget = target;
			return thisWrapper;
		}

		public virtual delegateWrapper getThisWrapper()
		{
			return null;
		}
	}
}

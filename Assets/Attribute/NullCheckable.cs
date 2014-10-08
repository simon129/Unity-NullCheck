using System;

[AttributeUsage(AttributeTargets.Class)]
public sealed class NullCheckable : Attribute
{
	public NullCheckable() { }
}
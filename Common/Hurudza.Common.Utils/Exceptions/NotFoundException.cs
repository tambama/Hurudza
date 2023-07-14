﻿namespace Hurudza.Common.Utils.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message)
        : base($"{message}")
    {
    }

    public NotFoundException(string name, object key)
        : base($"{name} ({key}) was not found.")
    {
    }
}

﻿using System.Collections;

namespace Squee.Antd.Pro;

public interface IStepsForm
{
    string Title { get; }
    object? Entity { get; }
    IStep[] Steps { get; }
}

public class StepsForm(string title, object? entity) : IStepsForm, IEnumerable<IStep>
{
    public string Title { get; set; } = title;
    public object? Entity { get; set; } = entity;

    private readonly List<IStep> _steps = [];
    public IStep[] Steps => [.. _steps];

    public void Add(IStep step)
    {
        _steps.Add(step);
    }

    public IEnumerator<IStep> GetEnumerator()
    {
        return _steps.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _steps.GetEnumerator();
    }
}

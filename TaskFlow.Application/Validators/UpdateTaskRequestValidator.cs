using FluentValidation;
using TaskFlow.Application.DTOs.Requests;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Validators;

public class UpdateTaskRequestValidator : AbstractValidator<UpdateTaskRequest>
{
    public UpdateTaskRequestValidator()
    {
        RuleFor(x => x.Title)
            .MaximumLength(500)
            .When(x => x.Title is not null);

        RuleFor(x => x.DueDate)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("Due date must be in the future")
            .When(x => x.DueDate.HasValue);

        RuleFor(x => x.Priority)
            .Must(x => Enum.IsDefined(typeof(AppTaskPriority), x!))
            .WithMessage("Priority must be a valid value (0=Low, 1=Medium, 2=High, 3=Critical)")
            .When(x => x.Priority.HasValue);

        RuleFor(x => x.Status)
            .Must(x => Enum.IsDefined(typeof(AppTaskStatus), x!))
            .WithMessage("Status must be a valid value (0=ToDo, 1=InProgress, 2=InReview, 3=Done)")
            .When(x => x.Status.HasValue);

        RuleFor(x => x.Description)
            .MaximumLength(5000)
            .When(x => x.Description is not null);
    }
}
using FluentValidation;
using TaskFlow.Application.DTOs.Requests;

namespace TaskFlow.Application.Validators;

public class UpdateTaskRequestValidator : AbstractValidator<UpdateTaskRequest>
{
    public UpdateTaskRequestValidator()
    {
        RuleFor(x => x.Title)
            .MaximumLength(500)
            .When(x => x.Title is not null);

        RuleFor(x => x.DueDate)
            .GreaterThan(DateTimeOffset.UtcNow)
            .WithMessage("Due date must be in the future")
            .When(x => x.DueDate.HasValue);

        RuleFor(x => x.Priority)
            .IsInEnum()
            .When(x => x.Priority.HasValue);

        RuleFor(x => x.Status)
            .IsInEnum()
            .When(x => x.Status.HasValue);

        RuleFor(x => x.Description)
            .MaximumLength(5000)
            .When(x => x.Description is not null);
    }
}
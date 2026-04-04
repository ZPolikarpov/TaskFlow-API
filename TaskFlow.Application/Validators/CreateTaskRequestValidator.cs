using FluentValidation;
using TaskFlow.Application.DTOs.Requests;

namespace TaskFlow.Application.Validators;

public class CreateTaskRequestValidator : AbstractValidator<CreateTaskRequest>
{
    public CreateTaskRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.Priority)
            .NotEmpty()
            .IsInEnum();

        RuleFor(x => x.DueDate)
            .GreaterThan(DateTimeOffset.UtcNow)
            .WithMessage("Due date must be in the future")
            .When(x => x.DueDate.HasValue);

        RuleFor(x => x.ProjectId)
            .NotEmpty();

        RuleFor(x => x.Description)
            .MaximumLength(5000)
            .When(x => x.Description is not null);

        RuleFor(x => x.Status)
            .IsInEnum()
            .When(x => x.Status.HasValue);
    }
}
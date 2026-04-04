using FluentValidation;
using TaskFlow.Application.DTOs.Requests;

namespace TaskFlow.Application.Validators;

public class CreateProjectRequestValidator : AbstractValidator<CreateProjectRequest>
{
    public CreateProjectRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .When(x => x.Description is not null);
    }
}
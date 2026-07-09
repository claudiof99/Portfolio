# Domain Type Organization

To avoid duplication and confusion, domain types are organized as follows:

- `Entities/` → Aggregate roots and entities only.
- `Enums/` → Enum definitions only.
- `ValueObjects/` → Value-object definitions only.

## Canonical enum files

- `Enums/UserRole.cs`
- `Enums/ReviewStatus.cs`
- `Enums/PurchaseStatus.cs`
- `Enums/DurationUnit.cs`
- `Enums/PersonalListType.cs`
- `Enums/AwardCategory.cs`
- `Enums/CartStatus.cs`
- `Enums/SessionType.cs`

## Canonical value object files

- `ValueObjects/Genre.cs`
- `ValueObjects/CreditFilm.cs`
- `ValueObjects/Person.cs`
- `ValueObjects/Duration.cs`
- `ValueObjects/PurchaseItemValue.cs`
- `ValueObjects/DateRange.cs`
- `ValueObjects/Money.cs`

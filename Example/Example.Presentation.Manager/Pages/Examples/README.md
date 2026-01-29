# Dashboard Example Pages

This directory contains example pages showcasing common dashboard layouts and UI patterns. Use these as templates when building new pages.

---

## Available Examples

| Page | Route | Description |
|------|-------|-------------|
| **Index** | `/examples` | Gallery of all example pages |
| **Dashboard** | `/examples/dashboard` | Analytics overview with stats, charts, activity |
| **Users List** | `/examples/users` | Data table with search, filters, pagination |
| **User Form** | `/examples/users/create` | Create/edit form with validation |
| **User Details** | `/examples/users/{id}` | Profile page with activity and stats |
| **Settings** | `/examples/settings` | Multi-section settings page |
| **Profile** | `/examples/profile` | User profile with cover, stats, social |
| **Error 404** | `/examples/error-404` | Not Found error page |
| **Error 403** | `/examples/error-403` | Forbidden error page |
| **Error 500** | `/examples/error-500` | Server Error page |

---

## Layout Patterns

### 1. Dashboard / Analytics

**File:** [Dashboard.razor](Dashboard.razor)

Best for: Home pages, analytics, overviews

```
┌─────────────────────────────────────────────────────┐
│ Stats Cards (4 columns)                             │
├─────────────────────────────────────────────────────┤
│ Chart Area              │ Activity Timeline         │
│ (8 cols)                │ (4 cols)                  │
├─────────────────────────────────────────────────────┤
│ Data Table (full width)                             │
└─────────────────────────────────────────────────────┘
```

**Key Components:**
- `MudGrid` with `MudItem` for responsive columns
- `MudPaper` with `Elevation="0"` and border style
- Stat cards with trend indicators
- Timeline for activity

---

### 2. Data Table / List

**File:** [UsersList.razor](UsersList.razor)

Best for: CRUD lists, data management

```
┌─────────────────────────────────────────────────────┐
│ Header: Title + Action Buttons                      │
├─────────────────────────────────────────────────────┤
│ Filters: Search | Role Filter | Status Filter      │
├─────────────────────────────────────────────────────┤
│ MudTable                                            │
│ ┌─────┬──────────┬────────┬─────────┬─────────────┐ │
│ │ ☐   │ User     │ Role   │ Status  │ Actions     │ │
│ ├─────┼──────────┼────────┼─────────┼─────────────┤ │
│ │ ...data rows...                                 │ │
│ └─────────────────────────────────────────────────┘ │
│ Pagination                                          │
└─────────────────────────────────────────────────────┘
```

**Key Components:**
- `MudTable` with `ServerData` for pagination
- `MudTextField` with `Adornment` for search
- `MudSelect` for filters
- `MudChip` for status badges
- Bulk action toolbar

---

### 3. Form / Create-Edit

**File:** [UserForm.razor](UserForm.razor)

Best for: Create/Edit forms, data entry

```
┌─────────────────────────────────────────────────────┐
│ Breadcrumbs                                         │
├─────────────────────────────────────────────────────┤
│ Header: Title + Action Buttons                      │
├─────────────────────────────────────────────────────┤
│ Section: Basic Information                          │
│ ┌─────────────────┬─────────────────────────────┐   │
│ │ Avatar Upload   │ Name, Email, Phone fields   │   │
│ │ (4 cols)        │ (8 cols)                    │   │
│ └─────────────────┴─────────────────────────────┘   │
├─────────────────────────────────────────────────────┤
│ Section: Role & Permissions                         │
│ ┌───────────────────────────────────────────────┐   │
│ │ Role Select | Permissions Checkboxes          │   │
│ └───────────────────────────────────────────────┘   │
├─────────────────────────────────────────────────────┤
│ Footer: Cancel | Save Buttons                       │
└─────────────────────────────────────────────────────┘
```

**Key Components:**
- `MudBreadcrumbs` for navigation
- `MudForm` with validation
- Sectioned `MudPaper` cards
- `MudFileUpload` for avatar
- `MudCheckBox` for permissions

---

### 4. Details / Profile

**File:** [UserDetails.razor](UserDetails.razor)

Best for: Detail views, profiles, dashboards

```
┌─────────────────────────────────────────────────────┐
│ Profile Header                                      │
│ ┌────────┐                                          │
│ │ Avatar │ Name, Role, Status, Actions              │
│ └────────┘                                          │
├───────────────────────────────┬─────────────────────┤
│ Main Content (8 cols)         │ Sidebar (4 cols)    │
│ ┌───────────────────────────┐ │ ┌─────────────────┐ │
│ │ About Section             │ │ │ Statistics      │ │
│ ├───────────────────────────┤ │ ├─────────────────┤ │
│ │ Activity Timeline         │ │ │ Permissions     │ │
│ ├───────────────────────────┤ │ ├─────────────────┤ │
│ │ Projects List             │ │ │ Sessions        │ │
│ └───────────────────────────┘ │ └─────────────────┘ │
└───────────────────────────────┴─────────────────────┘
```

**Key Components:**
- Profile header with avatar and actions
- Two-column layout (content + sidebar)
- `MudTimeline` for activity
- `MudSimpleTable` for data
- `MudChip` for tags/badges

---

### 5. Settings

**File:** [Settings.razor](Settings.razor)

Best for: Application settings, preferences

```
┌──────────────┬──────────────────────────────────────┐
│ Navigation   │ Content Area                         │
│ (3 cols)     │ (9 cols)                             │
│ ┌──────────┐ │ ┌──────────────────────────────────┐ │
│ │ General  │ │ │ Section: General                 │ │
│ │ Appear.  │ │ │ Organization Name, Website...   │ │
│ │ Notific. │ │ ├──────────────────────────────────┤ │
│ │ Security │ │ │ Section: Appearance              │ │
│ │ Integr.  │ │ │ Theme Cards, Switches...         │ │
│ │ Danger   │ │ ├──────────────────────────────────┤ │
│ └──────────┘ │ │ Section: Danger Zone             │ │
│              │ │ Delete Account button            │ │
│              │ └──────────────────────────────────┘ │
└──────────────┴──────────────────────────────────────┘
```

**Key Components:**
- Navigation sidebar with anchor links
- Sectioned content with IDs
- `MudSwitch` for toggles
- `MudRadioGroup` for theme selection
- Danger zone with destructive actions

---

### 6. Profile (Social)

**File:** [Profile.razor](Profile.razor)

Best for: User profiles, public pages

```
┌─────────────────────────────────────────────────────┐
│ Cover Photo (full width, 200px height)              │
├─────────────────────────────────────────────────────┤
│ ┌────────┐ Name, Title                              │
│ │ Avatar │ Location, Stats Row                      │
│ └────────┘ Action Buttons                           │
├───────────────────────────────┬─────────────────────┤
│ Main Content (8 cols)         │ Sidebar (4 cols)    │
│ ┌───────────────────────────┐ │ ┌─────────────────┐ │
│ │ About                     │ │ │ Contact Info    │ │
│ │ Skills (Chips)            │ │ │ Social Links    │ │
│ │ Recent Projects (Grid)    │ │ │ Team Members    │ │
│ │ Activity Timeline         │ │ └─────────────────┘ │
│ └───────────────────────────┘ │                     │
└───────────────────────────────┴─────────────────────┘
```

**Key Components:**
- Cover photo with gradient overlay
- Large avatar with offset position
- Stats row with numbers
- Skill chips
- Project cards in grid
- Team member avatars

---

### 7. Error Pages

**Files:** [Error404.razor](Error404.razor), [Error403.razor](Error403.razor), [Error500.razor](Error500.razor)

Best for: Error states, empty states

```
┌─────────────────────────────────────────────────────┐
│                                                     │
│              ┌─────────────────────┐                │
│              │    Error Icon       │                │
│              │    Error Code       │                │
│              │    Message          │                │
│              │    Description      │                │
│              │                     │                │
│              │    [Action Button]  │                │
│              │    [Search Field]   │ (404 only)     │
│              └─────────────────────┘                │
│                                                     │
└─────────────────────────────────────────────────────┘
```

**Key Components:**
- Centered layout with `d-flex justify-center align-center`
- Large icon with muted color
- Clear action buttons
- Search field (404)
- Technical details (500)

---

## Design Guidelines

### Card Styling

Use flat cards with subtle borders:

```razor
<MudPaper Elevation="0" Class="pa-6 rounded-lg" 
          Style="border: 1px solid var(--mud-palette-lines-default);">
    <!-- content -->
</MudPaper>
```

### Responsive Grid

Use MudGrid with xs/sm/md breakpoints:

```razor
<MudGrid>
    <MudItem xs="12" md="6" lg="3">
        <!-- Responsive column -->
    </MudItem>
</MudGrid>
```

### Consistent Spacing

- Page padding: `pa-4` or `pa-6`
- Section margins: `mb-4` or `mb-6`
- Card padding: `pa-4` or `pa-6`
- Stack spacing: `Spacing="3"` or `Spacing="4"`

### Color Usage

| Purpose | Color |
|---------|-------|
| Primary actions | `Color.Primary` |
| Success states | `Color.Success` |
| Warnings | `Color.Warning` |
| Errors | `Color.Error` |
| Info | `Color.Info` |
| Muted text | `Color.Secondary` |

### Status Chips

```razor
<MudChip T="string" Size="Size.Small" Color="@GetStatusColor(status)">
    @status
</MudChip>
```

---

## File Naming Convention

| Type | Pattern | Example |
|------|---------|---------|
| List page | `{Entity}List.razor` | `UsersList.razor` |
| Form page | `{Entity}Form.razor` | `UserForm.razor` |
| Details page | `{Entity}Details.razor` | `UserDetails.razor` |
| Settings | `Settings.razor` | `Settings.razor` |
| Dashboard | `Dashboard.razor` | `Dashboard.razor` |
| Error pages | `Error{Code}.razor` | `Error404.razor` |

---

## Route Convention

```csharp
@page "/examples"                    // Index
@page "/examples/dashboard"          // Dashboard
@page "/examples/users"              // List
@page "/examples/users/create"       // Create form
@page "/examples/users/{Id:guid}"    // Details
@page "/examples/users/{Id:guid}/edit" // Edit form
@page "/examples/settings"           // Settings
```

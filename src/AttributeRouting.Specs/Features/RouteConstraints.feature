﻿Feature: Route Constraints

Scenario: Regex route constraints specified with an attribute
	Given I generate the routes defined in the subject controllers
	When I fetch the routes for the RouteConstraints controller's Index action
	Then the parameter "p1" is constrained by the pattern "\d+"
	When I fetch the routes for the ApiRouteConstraints controller's Get action
	Then the parameter "p1" is constrained by the pattern "\d+"

Scenario: Regex route constraints specified inline
	Given I generate the routes defined in the subject controllers
	When I fetch the routes for the RouteConstraints controller's InlineConstraints action
	Then the route url is "InlineConstraints/{number}/{word}/{alphanum}/{capture}"
	Then the parameter "number" is constrained by the pattern "\d+"
	Then the parameter "word" is constrained by the pattern "\w{2}"
	Then the parameter "alphanum" is constrained by the pattern "[A-Za-z0-9]*"
	Then the parameter "capture" is constrained by the pattern "(gotcha)"
	When I fetch the routes for the HttpRouteConstraints controller's InlineConstraints action
	Then the route url is "InlineConstraints/{number}/{word}/{alphanum}/{capture}"
	Then the parameter "number" is constrained by the pattern "\d+"
	Then the parameter "word" is constrained by the pattern "\w{2}"
	Then the parameter "alphanum" is constrained by the pattern "[A-Za-z0-9]*"
	Then the parameter "capture" is constrained by the pattern "(gotcha)"

Scenario Outline: Inline constraints
	Given I generate the routes defined in the subject controllers
	# MVC
	When I fetch the routes for the InlineRouteConstraints controller's <actionName> action
	Then the route url is "Inline-Constraints/<actionName>/{x}"
	And the parameter "x" is constrained by an inline AttributeRouting.Web.Constraints.<constraintTypeName>
	# Web API
	When I fetch the routes for the HttpInlineRouteConstraints controller's <actionName> action
	Then the route url is "Http-Inline-Constraints/<actionName>/{x}"
	And the parameter "x" is constrained by an inline AttributeRouting.Web.Constraints.<constraintTypeName>
	Examples: 
	| actionName  | constraintTypeName       |
	| Alpha       | AlphaRouteConstraint     |
	| Int         | IntRouteConstraint       |
	| Long        | LongRouteConstraint      |
	| Float       | FloatRouteConstraint     |
	| Double      | DoubleRouteConstraint    |
	| Decimal     | DecimalRouteConstraint   |
	| Bool        | BoolRouteConstraint      |
	| Guid        | GuidRouteConstraint      |
	| DateTime    | DateTimeRouteConstraint  |
	| Length      | LengthRouteConstraint    |
	| MinLength   | MinLengthRouteConstraint |
	| MaxLength   | MaxLengthRouteConstraint |
	| LengthRange | LengthRouteConstraint    |
	| Min         | MinRouteConstraint       |
	| Max         | MaxRouteConstraint       |
	| Range       | RangeRouteConstraint     |
	| Regex       | RegexRouteConstraint     |
	| Compound    | IntRouteConstraint       |
	| Compound    | MaxRouteConstraint       |

Scenario: Multiple inline constraints per url segment
	# MVC
	Given I have registered the routes for the InlineRouteConstraintsController
	When I fetch the routes for the InlineRouteConstraints controller's MultipleWithinUrlSegment action
	Then the route url is "Inline-Constraints/avatar/{width}x{height}/{image}"
	And the parameter "width" is constrained by an inline AttributeRouting.Web.Constraints.IntRouteConstraint
	And the parameter "height" is constrained by an inline AttributeRouting.Web.Constraints.IntRouteConstraint
	# Web API
	Given I have registered the routes for the HttpInlineRouteConstraintsController
	When I fetch the routes for the HttpInlineRouteConstraints controller's MultipleWithinUrlSegment action
	Then the route url is "Http-Inline-Constraints/avatar/{width}x{height}/{image}"
	And the parameter "width" is constrained by an inline AttributeRouting.Web.Constraints.IntRouteConstraint
	And the parameter "height" is constrained by an inline AttributeRouting.Web.Constraints.IntRouteConstraint

Scenario Outline: Matching inline route constraints
	# MVC
	Given I have registered the routes for the InlineRouteConstraintsController
	When a request for "Inline-Constraints/<url>" is made
	Then the <action> action <condition> matched
	# Web API
	Given I have registered the routes for the HttpInlineRouteConstraintsController
	When a request for "Http-Inline-Constraints/<url>" is made
	Then the <action> action <condition> matched
	Examples:
	| url                                       | action       | condition |
	| Alpha/abc                                 | Alpha        | is        |
	| Alpha/123                                 | Alpha        | is not    |
	| Int/53                                    | Int          | is        |
	| Int/abc                                   | Int          | is not    |
	| IntOptional                               | IntOptional  | is        |
	| Long/79                                   | Long         | is        |
	| Long/xyz                                  | Long         | is not    |
	| Float/1.334                               | Float        | is        |
	| Float/gg2                                 | Float        | is not    |
	| Double/3.14                               | Double       | is        |
	| Double/adf78                              | Double       | is not    |
	| Decimal/99.32123345                       | Decimal      | is        |
	| Decimal/d8uasdf                           | Decimal      | is not    |
	| Bool/true                                 | Bool         | is        |
	| Bool/false                                | Bool         | is        |
	| Bool/truish                               | Bool         | is not    |
	| Bool/falsish                              | Bool         | is not    |
	| Guid/6076668C-57AA-47FD-A914-94FD552C8493 | Guid         | is        |
	| Guid/6076668C-57AA-47FD-A914-94FD552C     | Guid         | is not    |
	| DateTime/2012-4-22                        | DateTime     | is        |
	| DateTime/Today                            | DateTime     | is not    |
	| Length/a                                  | Length       | is        |
	| Length/aa                                 | Length       | is not    |
	| MinLength/abcdefghi                       | MinLength    | is not    |
	| MinLength/abcdefghij                      | MinLength    | is        |
	| MaxLength/abcdefghij                      | MaxLength    | is        |
	| MaxLength/abcdefghijk                     | MaxLength    | is not    |
	| LengthRange/abcdefghijk                   | LengthRange  | is not    |
	| LengthRange/a                             | LengthRange  | is not    |
	| LengthRange/ab                            | LengthRange  | is        |
	| LengthRange/abcdefghij                    | LengthRange  | is        |
	| LengthRange/abcdefghijk                   | LengthRange  | is not    |
	| Min/0                                     | Min          | is not    |
	| Min/1                                     | Min          | is        |
	| Max/10                                    | Max          | is        |
	| Max/11                                    | Max          | is not    |
	| Range/0                                   | Range        | is not    |
	| Range/1                                   | Range        | is        |
	| Range/10                                  | Range        | is        |
	| Range/11                                  | Range        | is not    |
	| Regex/Howdy                               | Regex        | is        |
	| Regex/BoyHowdy                            | Regex        | is not    |
	| Compound/5                                | Compound     | is        |
	| Compound/5.0                              | Compound     | is not    |
	| Enum/red                                  | Enum         | is        |
	| Enum/taupe                                | Enum         | is not    |
	| WithOptional                              | WithOptional | is        |
	| WithDefault                               | WithDefault  | is        |

Scenario: Multiple routes with different constraints
	Given I generate the routes defined in the subject controllers
	When I fetch the routes for the RouteConstraints controller's MultipleRoutes action
	Then the route named "MultipleConstraints1" has a constraint on "p1" of "\d+"
	And the route named "MultipleConstraints2" has a constraint on "p1" of "\d{4}" 
	And the route named "ApiMultipleConstraints1" has a constraint on "p1" of "\d+"
	And the route named "ApiMultipleConstraints2" has a constraint on "p1" of "\d{4}"
	When I fetch the routes for the HttpRouteConstraints controller's MultipleRoutes action
	Then the route named "MultipleConstraints1" has a constraint on "p1" of "\d+"
	And the route named "MultipleConstraints2" has a constraint on "p1" of "\d{4}" 
	And the route named "ApiMultipleConstraints1" has a constraint on "p1" of "\d+"
	And the route named "ApiMultipleConstraints2" has a constraint on "p1" of "\d{4}"

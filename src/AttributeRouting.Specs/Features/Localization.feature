Feature: Localization

Scenario: Using the translations feature requires a configured translation provider
	Given I generate the routes defined in the subject controllers
	 When I fetch the routes for the Localization controller's Index action
	 Then the route has no constraint with the key "currentUICultureName"

Scenario Outline: Set the translation keys for the translatable properties of routing attributes
	Given I have a new configuration object
	  And I add the routes from the subject controllers
	  And I configure a new TestTranslationProvider with:
		| key							| value		| cultureName	|
		| Localization_Index_RouteUrl	| Index		| en			|
	  And I generate the routes with this configuration
	 When I fetch the routes for the <controller> controller's <action> action
	 Then the <Nth> route's data token for "<translationKeyName>" is "<translationKeyValue>"

	Examples:
		| controller					| action					| Nth	| translationKeyName			| translationKeyValue								|
		| Localization					| Index						| 1st	| RouteUrlTranslationKey		| Localization_Index_1_RouteUrl						|
		| Localization					| Index						| 2nd	| RouteUrlTranslationKey		| Localization_Index_2_RouteUrl						|
		| Localization					| ExplicitTranslationKey	| 1st	| RouteUrlTranslationKey		| Localization_Explicit_RouteUrl					|
		| AreaLocalization				| Index						| 1st	| AreaUrlTranslationKey			| AreaLocalization_AreaUrl							|
		| AreaLocalizationExplicitKey	| Index						| 1st	| AreaUrlTranslationKey			| Explicit_AreaUrl									|
		| PrefixLocalization			| Index						| 1st	| RoutePrefixUrlTranslationKey	| PrefixLocalization_RoutePrefixUrl					|
		| PrefixLocalizationExplicitKey	| Index						| 1st	| RoutePrefixUrlTranslationKey	| Explicit_RoutePrefixUrl							|
		| PrefixedAreaLocalization		| Index						| 1st	| RoutePrefixUrlTranslationKey	| Area_PrefixedAreaLocalization_RoutePrefixUrl		|

Scenario: Register translated routes
	Given I have a new configuration object
	  And I add the routes from the Localization controller
	  And I configure a new TestTranslationProvider with:
		| key							| value		| cultureName	|
		| Localization_Index_RouteUrl	| Index		| en			|
	  And I generate the routes with this configuration
	 When I fetch the routes for the Localization controller's Index action
	 Then 2 routes are found
	  And each route has a constraint with the key "currentUICultureName"

Scenario Outline: Constrain outbound route generation by the current UI culture
	Given I have a new configuration object
	  And I add the routes from the Localization controller
	  And I configure a new TestTranslationProvider with:
		| key							| value		| cultureName	|
		| Localization_Index_1_RouteUrl	| Index		| en			|
		| Localization_Index_1_RouteUrl	| Indice	| es			|
		| Localization_Index_2_RouteUrl	| Index2	| en			|
		| Localization_Index_2_RouteUrl	| Indice2	| es			|
	  And I generate the routes with this configuration
	  And I set the current thread's CurrentUICulture to "<cultureName>"
	 When I generate the url for the Localization controller's Index action
	 Then the generated url is "/<url>"
	
	Examples:
		| cultureName	| url		|
		| en-US			| Index		|
		| en			| Index		|
		| fr-FR			| Index		|
		| fr			| Index		|
		| es-ES			| Indice	|
		| es			| Indice	|

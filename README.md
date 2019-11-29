# BLS
# This library is in active development
BLS is a library aimed to provide support in developing application business logic in C#. Much like the famous front-end frameworks such as Angular or React, BLS's main goal is to provide a structure on which backend developers would be able to build a reliable and maintainable business model. While still in active development, the framework is being build to provide the user with the following features:
 - you build your business entities with simple, inheritable POCO classes all deriving from the BlsPawn class.
 - you build relations between the business entities using Relations - special properties on pawns to enable traversing between business objects
 - you register the business model with the BLS which validates that all relations are correct (that there are no duplicate relations for example, see BlGraph tests for details)
 - BLS provides a storage solution (through the IStorageProvider interface) but you do not have to work with it directly. You simply retrieve business objects from the storage, change their properties, update their relations and save the changes. BLS transactionally persists the changes to the storage.
 
Working with BLS should be similar to working with Entity Framework except BLS tries to abstract away even more DB stuff from the developer

NHibernate Deep Dive
================

This repo contains code samples originally prepared for an internal company presentation. Code samples are prepared in a form of unit tests, each presenting a particular aspect of NHibernate library. Tests are partitioned into categories covering major features of NHibernate

## How to use the code?

Just run all the tests in the solution and browse the results. The names of tests are self-explanatory. You will need a database to run the samples. The code is pre-configured to run against a default local instance of Microsoft SQL Server 2008. It can be reconfigured, however, to run against any RDBMS supported by NHibernate.

## The basics

The first thing you learn about NHibernate is that it supports basic CRUD (Create, Read, Update, Delete) operations.

All the examples are based on the same set of classes forming a simple domain model of customers and orders. There are customers each having a collection of orders. Orders can be categorized -- each order can be assigned 0 or more categories. Customers come in 2 flavors: there are preferred customers and bulk customers.

## Inheritance

Inheritance is one the major cause of so-called Object-Relational Mismatch. Relational world does not have a concept of inheritance so it must be emulated on top of relational concepts like tables, primary and foreign keys and relations.

There are three primary strategies for implementing inheritance on top of relational store and all of them are fully supported by NHibernate. Each of them has both advantages and disadvantages so when in need of using inheritance, make a conscious decission.

### Table per class hierarchy

This strategy is the simplest from the relational schema point of view. There is only one table for the whole inheritance hierarchy and it stores all the objects. The concrete classes of objects are stored in a column called *discriminator column* in a customizable form (an integral value, a name of even a fully qualified class name). Using one table to store all kinds of objects requires that columns specific to one concrete class are *nullable*. From relational purity point of view, this is of course a major drawback. On the other hand, using single table make polymorphic queries (queries for abstract classes, returning instances of various concrete classes) easy to implement and performant at runtime (no joins required).

### Table per class

This strategy uses one table for each class in the hierarchy, no matter if the class is abstract or concrete. The effect is, when searching for objects of particular class, at least 2 (parent and child) tables must be joined to obtain a complete result set. If the inheritance chain is longer then 2 elements, there would be more joins. This obviously complicates greatly implementing polymorphic queries. To be fair, table per class hierarchy has also some advantages. The most notable is it results in an elegant relational model which can be easily understood by itself.

### Table per concrete class

This is a compromise between the former two strategies. It uses a distinct table for each concrete (leaf) class in the inheritance tree. If retains some advantages of table per hierarchy (polymorphic queries are relatively simple, requiring only `UNION` SQL clause) while resulting in a fairly clean (no nullable columns) relational model.

## Second level cache

Second level cache is one of the most powerful (and distinguishing) features of NHibernate yet is it also one of the least known. What is it? Second level cache allows to share object instances between NHibernate sessions. Once objects are loaded into memory, they are available for all threads without need to go to database.

For second level cache to work, it must be explicitly enable. Two additional requirements must also be met, namely:

* A session must not use custom-provided connection
* All updates must be performed inside an explicitly opened NHibernate transaction (`ITransaction`)

### Entity cache

The entity cache stores whole entities (objects). Objects in cache are reachable by their IDs so the roundtrip to database is saved only when searching by ID. Otherwise, a lookup in the database is performed, but the actual entities are retrieved from cache. Entity cache must be enabled explicitly in the mapping file for each entity type that is going to be cached.

### Collection cache

The collection cache stores identifiers (and **only** identifiers) of objects contained in collections referenced by other objects. When hydrating a parent object from database, if its collection is cached, IDs of child objects are retrieved from the cache and NHibernate retrieves the child objects **one by one** from the database. This means that enabling collection cache can actually degrade performance instead of boosting it.

What you want to do is probably enable also entity caching for objects contained in the cached collection. Only this way the whole collection (both object IDs and their data) will be stored in memory and a database roundtrip will be saved.

### Query cache

The query cache works similar to the collection cache -- it also stores only identifiers of objects returned by a query. For query cache to work, it must be enabled globally (on `Configuration` object) and then locally for each query that is to be cached.

The same performance problem applies to query cache also. If used without caching actual entities, it can degrade performance significantly.

## Concurrency

NHibernate offers several concurrency handling strategies.They can be broken down into two well-known categories: optimistic and pessimistic concurrency.

### Optimistic concurrency (simple)

This strategy uses all known (after loading an entity into memory) field values as version information. When issuing an update, all the original field values are appended to the where clause of update statement. This results in performing actual update only if no single field in the database has changed its value since the object has been loaded. NHibernate checks the number of affected rows and if it is zero (meaning where clause returned no results -- the data has changed) it throws a `StaleObjectStateException`.

Notice that (like in case of any other exception thrown by session object) a session that has encountered a version conflict becomes unusable and needs to be disposed immediately (and possibly replaced by a new one).

### Versioned optimistic concurrency

This strategy uses an integral version number which is incremented on each modification to the object. Version number is stored in a special column. When updating an object, NHibernate appends additional set clause to increment the version number field and an additional where clause with version number as it was at the time when object was retrieved from the database. The interpretation of the results of an update is similar to simple optimistic concurrency strategy

### Timestamped optimistic concurrency

This strategy is very similar to the former one, but uses a `DATETIME` value instead of an integral number. Due to the limited precision of date values there is a small probability that the new modification timestamp would be equal to the previous one resulting in an undetected version conflict. Because of this flaw this strategy is generally discouraged.

### Natively timestamped optimistic concurrency

Some databases, like Microsoft SQL Server support natively timestamp values which uniqueness is guaranteed by the database transactional engine.NHibernate can take advantage of this capabilities. The drawback of this strategy is that the native timestamp value is not human-readable and does not have any well-defined meaning to the problem domain.

### Pessimistic concurrency

While optimistic concurrency is about detecting version conflicts, pessimistic concurrency is about preventing them altogether. It does so by locking database rows when retrieving entities from the database. No other session can access the same object until the lock is released.

While whole books can be written on pros and cons of both optimistic and pessimistic concurrency, the rule of thumb is, use optimistic by default and pessimistic only if version conflicts cause major problems (in terms of amount of work that need to be redone).

## Session management

Session is one of the fundamental concepts of NHibernate. What is it? Some think of it as an object-oriented wrapper around a database connection, but it is only half of the truth. NHibernate session has numerous responsibilities and connection management is only one of them. The other are implementation of Unit of Work and Identity Map patterns.

### Unit of Work

[Unit of Work](http://martinfowler.com/eaaCatalog/unitOfWork.html) is one of Fowler's Enterprise Patterns. It keeps track of what data was changed since objects has been retrieved from database. Upon completion, Unit of Work flushes all the changes to the database in one batch. Session automatically track all the objects that were retrieved by it. That is why you don't have to call `ISession.Update()` explicitly on all modified objects. When calling `ISession.Flush()` or committing the session's transaction, NHibernate picks up all the modifications you made and sends them to the database.

### Identity Map

[Identity Map](http://martinfowler.com/eaaCatalog/identityMap.html) is another pattern described by Fowler implemented by NHibernate's session. Identity Map ensures that, in scope of one particular session, each row in the database corresponds to at most one object instance returned by the session. It means that is you get object with particular id twice, you will get the exact same instance both times. This guarantee is vital to avoid conflicting changes, where two objects representing the same row would be modified independently and send to the database as part of the same commit.

It is also worth notice that session is serializable. It is another argument that it is not a stupid wrapper around a database connection. Sessions can be temporarily disconnected from database and serialized to a byte array. Later, they can be deserialized, reconnected and function correctly.

### Session per call

This is the simplest session management strategy. For each call to the database layer, a new session is created. The corollary of this is you must provide a mechanism to communicate changes between these sessions.

One possibility is to use DTOs (Data Transfer Objects) in the upper layers. After obtaining an entity from the first session, it is mapped to a DTO. The DTO is modified (possibly by the user via user interface) and send back to the data layer. The second session retrieves the same entity, modifies its fields with values from provided DTO and persists the changes.

The second option is to use the same entity instance in all layers. In this case an entity needs to be detached (`ISession.Evict()`) from the first session and then, after performing modifications, reattached (`ISession.Update()`) to the second session.

### Session per request (ambient)

The more complex strategy is having exactly one session per each requests. Requests can be of various types: HTTP requests in a web application, WCF service calls and processing queued messages. The principle is always the same: session is created at the beginning of the request and disposed at the end. In the meantime, it is bound to some ambient context which allows it to be reachable from anywhere in the codebase.

The ambient context implementation is specific to the actual request type that is used. NHibernates supports out-of-the-box all the common scenarios providing ASP.NET, WCF and plain `ThreadStatic` ambient contexts.

### Session per... session (durable)

This strategy uses session serializability capabilities to provide a single Identity Map and Unit of Work scope throughout multiple requests. Between requests the session is stored in some persistent store, like ASP.NET session.

While this approach may seem to be more comfortable from the developer perspective, is has some major drawbacks, two of which are:
* it requires maintaining state on server side which always degrades scalability potential of the solution
* entities retrieved by the session are serialized along with the session. When large object graphs are retrieved, the serialized session size can be *huge*.

### Session per form

This approach is used commonly in smart client applications and is *the preferred approach*. The session is open for each form (or presenter or view model, depends on which variation of MVC pattern you are using). Because the form can be displayed for some time and user interaction can result in multiple transactions, there is a need for replacing the session after an exception was thrown (like `StateObjectStateException`).
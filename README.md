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
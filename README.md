# CDM (Canonical Data Model) Query API

To be used as a query abstraction layer over the Data Mart.

It should be used by API's in the Adapter Layer to service upstream University systems.

# GraphQL

To facilitate a flexible querying approach to the API, [GraphQL](https://graphql.org) has been adopted.

The GraphQL standard was created by Facebook and is widely adopted with an active community.

> GraphQL is a query language for APIs and a runtime for fulfilling those queries with your existing data. GraphQL provides a complete and understandable description of the data in your API, gives clients the power to ask for exactly what they need and nothing more, makes it easier to evolve APIs over time, and enables powerful developer tools.

# Example GraphQL Queries

The solution includes [middleware](https://datamodelqueryapi.asewerapidev.ad.mmu.ac.uk/ui/playground) at e.g. /ui/playground to run GraphQL queries against the API (it's a bit like Swagger on steriods)

To return the first 5 person records:

```
{
  person(pageSize:5) {
    totalCount,
    items {
      mmuId,
      givenName,
      dateOfBirth,
      surname,
      communications {
        mobilePhone
      }
    },
    pageInfo {
      hasNextPage,
      hasPreviousPage
    }
  }
}
```

#Paging

Use `pageSize` and `page` to use pagination. Note that `pageSize` is mandatory.

```
person(page:1,pageSize:5)
```

This can be used with the `totalCount` and `pageInfo` response properties to provide full offset paging capability on the client.

# Filtering

A `filter` parameter can be used to filter the results. Note this supports an array of filters, so more than one filter can be used.

```
person(filter:[{field:"isStaff",value:"1"}])
```

Would run the equivalent query of `FROM Person WHERE isStaff = 1`

# Ordering

An `orderBy` parameter can be used to order the results.

```
person(orderBy:"mmuId")
```
Would run the equivalent query of `FROM Person ORDER BY MmuId`

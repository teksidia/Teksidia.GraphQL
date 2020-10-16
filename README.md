# GraphQL Query API

The GraphQL standard was created by Facebook and is widely adopted with an active community.

> GraphQL is a query language for APIs and a runtime for fulfilling those queries with your existing data. GraphQL provides a complete and understandable description of the data in your API, gives clients the power to ask for exactly what they need and nothing more, makes it easier to evolve APIs over time, and enables powerful developer tools.

# Example GraphQL Queries

The solution includes middleware at e.g. /ui/playground to run GraphQL queries against the API (it's a bit like Swagger on steriods)

To return the first 5 records:

```
{
  student(pageSize:5) {
    totalCount,
    items {
      id,
      surname
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
student(filter:[{field:"id", op: "=", value:"1"}])
student(filter:[{field:"id", op: "in", value:"1,2,3"}])
```

Would run the equivalent query of `FROM Student WHERE isStaff = 1` and `FROM Student WHERE isStaff IN (1,2,3)`

# Ordering

An `orderBy` parameter can be used to order the results.

```
person(orderBy:"id")
```
Would run the equivalent query of `FROM Student ORDER BY Id`

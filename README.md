# Fauxtomer

Fauxtomer is a REST based Web API that allows you to generate test customer data for Swedish customers.

## Introduction

Testing systems that deal with customer data is never easy; since using real customer data can put you in legal trouble and generate badwill from customers if they would to find out,
ensuring that valid, coherent test data exists is a vital part in a good test environment.

## The Fauxtomer API

Using the Fauxtomer API, you can generate a large quantity of plausable customer data that should be able to pass for valid data in many cases. Depending on how thorough your data
validation is, there could be issues with some properties.

# Current state

 - [x] Valid test personal numbers from Swedish Tax Agenxy
 - [x] Plausable first and last names generated using names with 1 000 or more bearers
 - [x] Valid e-mail addresses generated using disposable e-mail services (Mailinator etc)
 - [ ] Valid phone numbers generated using unallocated number series and approved test data from the Swedish Post and Telecom Authority
 
# Limitations

* Uniqueness of data between customers cannot be guaranteed. A single customer instance should have valid data but duplicate data could exist between multiple instances
* Street addresses and phone numbers comply with the standard format, but any address validation or a more comprehesive phone validation would likely fail

TBC

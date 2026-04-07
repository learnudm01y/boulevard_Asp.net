# Social Impact Formula Explanation Report
**Date:** April 6, 2026  
**Language:** English  
**Source:** Based on the Social Impact Tracker formulas and supporting calculations.

---

## 1) Charity — Formula 5
**Formula:**

```text
Charity = Total Sales × Category Contribution × 10%
```

### Explanation
This formula calculates the charitable amount generated from sales.

- **Total Sales** = the total purchase amount or revenue.
- **Category Contribution** = the percentage contribution of the selected category to the local economy.
- **10%** = the fixed donation percentage.

### Example
If total sales are **500 AED** and category contribution is **15%**:

```text
Charity = 500 × 0.15 × 0.10
         = 7.5 AED
```

---

## 2) Created 10 Working Hours for our kids — Formula 1
**Formula:**

```text
Working Hours = Total Purchase / 50
```

### Explanation
According to the source document:

- **100 AED = 2 working hours**
- Therefore **1 hour = 50 AED**

This means every **50 AED spent creates 1 working hour**.

### Example
For **500 AED**:

```text
Working Hours = 500 / 50
              = 10 hours
```

This is why the system displays:

```text
Created 10 Working Hours for our kids
```

---

## 3) Supported 7 Local Brands
**Formula:**

```text
Local Brands Supported = Number of Unique Brands Selected and Purchased
```

### Explanation
This is not a mathematical percentage formula.
It is a counting formula.

The system counts the number of **distinct local brands** included in the purchase.

### Example
If the customer buys from:

- Brand A
- Brand B
- Brand C
- Brand D
- Brand E
- Brand F
- Brand G

Then:

```text
Supported Local Brands = 7
```

---

## 4) Reduce 9.5 kg of Carbon Emissions — Formula 2
**Formula:**

```text
Carbon Reduction = (Total Purchase × Local Economy Contribution %) / 52.6
```

### Explanation
The document states:

```text
52.6 AED = 1 kg of Carbon Emissions
```

This means every **52.6 AED of local economic contribution** offsets **1 kg of carbon emissions**.

### Example
For **500 AED** with **100% local contribution**:

```text
Carbon Reduction = (500 × 1.0) / 52.6
                  = 9.5 kg
```

---

## 5) Strengthened our Economic Self-sufficiency — Formula 3
**Formula:**

```text
Economic Self-sufficiency % = Total Sales / 1.42 Trillion
```

### Explanation
This formula measures how much the purchase contributes to the total national economy benchmark.

- **1.42 Trillion AED** = reference GDP / economy benchmark
- **Total Sales** = total purchase value

### Example
For **1,000 AED**:

```text
Economic % = 1000 / 1,420,000,000,000
           = 0.0000000704%
```

This value is typically displayed as a percentage with rounding.

---

# Summary Table

| Metric | Formula |
|---|---|
| Charity | Total Sales × Category Contribution × 10% |
| Working Hours | Total Purchase / 50 |
| Local Brands | Count of unique local brands purchased |
| Carbon Reduction | (Total Purchase × Local %) / 52.6 |
| Economic Self-sufficiency | Total Sales / 1.42 Trillion |

---

# Technical Note
The formulas above are structured to be directly implemented in **ASP.NET / C# backend services** and frontend dashboards.


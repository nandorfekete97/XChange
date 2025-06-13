import { useEffect, useState } from 'react';
import { Form, Input, Button, Select } from 'antd';
const FormItem = Form.Item;

function CurrencyConverter() {
  const [currencies, setCurrencies] = useState([]);
  const [amountInEuro, setAmountInEuro] = useState(''); // User input for the amount in EUR
  const [selectedCurrencyRate, setSelectedCurrencyRate] = useState(null); // Stores the selected currency's exchange rate data
  const [convertedAmount, setConvertedAmount] = useState(''); // Stores the result of the conversion (EUR * rate)

  const getAllCurrencies = async () => {
    const response = await fetch(`/api/Currency/GetAllCurrencies`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
      },
    });

    if (response.ok) {
      const data = await response.json();
      setCurrencies(data.currencyModels);

      // length > 0 check ensures that we only call `fetchRateAndConvert` if the currency list isn't empty.
      // It also sets a default selected currency for the initial render, improving UX.
      if (data.currencyModels.length > 0) {
        fetchRateAndConvert(data.currencyModels[0].id);
      }
    } else {
      const data = await response.json();
      throw new Error(data);
    }
  };

  const fetchRateAndConvert = async (currencyId) => {
    const response = await fetch(
      `/api/Currency/GetLastCurrencyRatesByCurrencyIds?currencyIds=${currencyId}`,
      {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
        },
      }
    );

    if (response.ok) {
      const data = await response.json();

      console.log("data in fetchRateAndConvert: ", data);

      // The API seems to return an array of rate objects (maybe more than one for batch support).
      // We're assuming we requested only one currency ID, so we take the first item (`data[0]`).
      // That object includes the rate and the currency info (e.g., name, shortName).
      const rate = data[0];
      setSelectedCurrencyRate(rate); // Store the selected currency's rate for use in calculations

      if (!isNaN(amountInEuro) && amountInEuro !== '') {
        const result = parseFloat(amountInEuro) * rate.rate;
        setConvertedAmount(result.toFixed(2)); 
      } else {
        setConvertedAmount('');
      }
    } else {
      const data = await response.json();
      throw new Error(data);
    }
  };

  useEffect(() => {
    getAllCurrencies();
  }, []);

  const handleAmountChange = (e) => {
    const value = e.target.value;
    setAmountInEuro(value);

    if (
      selectedCurrencyRate && 
      !isNaN(value) &&
      value !== ''
    ) {
      const result = parseFloat(value) * selectedCurrencyRate.rate;
      setConvertedAmount(result.toFixed(2));
    } else {
      setConvertedAmount('');
    }
  };

  const handleCurrencyChange = (currencyId) => {
    fetchRateAndConvert(currencyId);
  };

  return (
    <div>
      <h2>Convert from EUR</h2>

      <Form label="Convert from EUR">
        <FormItem label="EUR">
          <Input
            type="text"
            value={amountInEuro}
            onChange={handleAmountChange}
            placeholder="Enter EUR amount"
          />
        </FormItem>

        <FormItem label="Select">
          <Select
            value={selectedCurrencyRate?.currencyModel?.id} // Show selected currency in dropdown
            onChange={handleCurrencyChange} // Fetch new rate when user selects a different currency
          >
            {currencies.map((currency) => (
              <Select.Option key={currency.id} value={currency.id}>
                {currency.name} ({currency.shortName})
              </Select.Option>
            ))}
          </Select>
        </FormItem>

        <FormItem label="Converted">
          <Input type="text" value={convertedAmount} readOnly />
        </FormItem>

        <FormItem>
          <Button type="submit">Submit</Button>
        </FormItem>
      </Form>
    </div>
  );
}

export default CurrencyConverter;

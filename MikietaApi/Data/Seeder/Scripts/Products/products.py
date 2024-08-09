
# This script is used to convert the XML file in a structure like below, into a JSON file.
# The JSON file is used to store the products in the database.

from bs4 import BeautifulSoup
import json

def GetXmlData(name):
  with open(name + '.xml', 'r', encoding='utf-8') as f:
    data = f.read()
  return BeautifulSoup(data, "html.parser")

def SaveJson(item):
  with open('products.json', 'w', encoding='utf-8') as f:
    json.dump(item, f, ensure_ascii=False, indent=4)

def ParsePizza():
    items = GetXmlData('pizzas').find_all('div', {'class': 'row'})
    pizzas = []
    for item in items:
      pizza = {
      "name": item.find('div', {'class': 'left'}).find('h2').text,
	  	"ingredients": item.find('div', {'class': 'left'}).find('h4').text,
	  	"medium_price": item.find('div', {'class': 'right'}).find_all('h2', {'class': 'price'})[0].text.strip().replace(",", "."),
	  	"large_price": item.find('div', {'class': 'right'}).find_all('h2', {'class': 'price'})[1].text.strip().replace(",", ".")
    }
      pizzas.append(pizza);
    return {"pizzas": pizzas}
    
def ParseOthers(name):
    items = GetXmlData(name).find_all('div', {'class': 'row'})
    res = []
    for item in items:
      element = {
      "name": item.find('div', {'class': 'left'}).find('h2').text,
    	"description": item.find('div', {'class': 'left'}).find('h4').text.strip(),
    	"price": item.find('div', {'class': 'right'}).find('h2', {'class': 'price'}).text.strip().replace(",", ".")
    }
      res.append(element);
    return {name: res}
  
def ParseDrinks():
    items = GetXmlData("drinks").find_all('div', {'class': 'row drink'})
    res = []
    for item in items:
      element = {
      "name": item.find('h2', {'class': 'left'}).text,
    	"price": item.find('h2', {'class': 'price'}).text.replace(",", ".").replace("zł", "").strip()
    }
      res.append(element);
    return {"drinks": res}  
  

print("Parsing...")

SaveJson({**ParsePizza(), **ParseOthers("dinners"), **ParseOthers("macarons"), **ParseOthers("salads"), **ParseOthers("deserts"), **ParseDrinks()});

print("Finished...")






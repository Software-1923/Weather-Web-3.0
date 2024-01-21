from flask import Flask, jsonify, request
import requests
from web3 import Web3
import os
from dotenv import load_dotenv

load_dotenv()

app = Flask(__name__)

alchemy_api_key = os.getenv("W0euO4lKSsktbMBH3QVnxMR2Vkbh5qIm")
api_url = f"https://eth-mainnet.alchemyapi.io/v2/{alchemy_api_key}"

opensea_nft_contract_address = "0x00000000000000ADc04C56Bf30aC9d3c0aAF14dC"
order_fulfilled_event_signature = "0x9d9af8e38d66c62e2c12f0225249fd9d721c54b83f48d9352c97c6cacdcb6f31"

web3 = Web3(Web3.HTTPProvider(api_url))


def get_latest_block_number():
    try:
        endpoint = f"?module=logs&action=getLogs&fromBlock=latest&toBlock=latest&address={opensea_nft_contract_address}&topic0={order_fulfilled_event_signature}"
        response = requests.get(api_url + endpoint)
        latest_block_number = response.json()["result"][0]["blockNumber"]
        print("Latest block number:", latest_block_number)
        return latest_block_number
    except Exception as e:
        print("Alchemy API error:", str(e))
        raise Exception("Error during Alchemy API request")


def handle_order_fulfilled_event(data):
    print("Received OrderFulfilled event data:", data)
    # Burada OrderFulfilled event'ını işleyebilirsiniz


@app.route("/block", methods=["GET"])
def get_block():
    try:
        latest_block_number = get_latest_block_number()
        return jsonify({"blockNumber": latest_block_number})
    except Exception as e:
        print("Error during /block endpoint:", str(e))
        return jsonify({"error": "Internal Server Error"}), 500


@app.route("/alchemy-webhook", methods=["POST"])
def alchemy_webhook():
    data = request.json
    # Gelen veri içinde OrderFulfilled event'ını işleyecek bir kontrol ekleyebilirsiniz
    if data and data.get("event") and data["event"].get("type") == "OrderFulfilled":
        handle_order_fulfilled_event(data)
    return "Webhook received successfully", 200


def main():
    try:
        latest_block_number = get_latest_block_number()
        print("Latest block number:", latest_block_number)

        # Diğer işlemlerinizi burada devam ettirebilirsiniz
    except Exception as e:
        print("Error during main function execution:", str(e))


if __name__ == "__main__":
    main()
    app.run(port=7052)

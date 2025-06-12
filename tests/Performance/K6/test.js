import http from 'k6/http';
import { check } from 'k6';
import { Counter } from 'k6/metrics';
import exec from 'k6/execution';


let orderCounter = new Counter('order_counter');

const INITIAL_ORDER_ID = 1;

export let options = {
    vus: 100, // usuários virtuais
    iterations: 200000, // total de requisições
    duration: '5h'
};

export default function () {
    //let orderId = (__VU * 1000000) + __ITER + INITIAL_ORDER_ID;
    //let orderId = exec.scenario.iterationInTest + INITIAL_ORDER_ID;
    //let orderId = Date.now() * 1000 + (__VU * 100) + __ITER;

    //let base = (__VU * 100000) + __ITER;
    //let orderId = base % 2147483647;

    //let orderId = Date.now() + (__VU * 1000) + Math.floor(Math.random() * 1000);

    //let orderId = (exec.scenario.iterationInTest % 2147483647) + 1;

    let orderId = Math.floor((Date.now() % 2147483647) + (__VU * 1000) + Math.random() * 1000);

    orderCounter.add(1);

    let payload = JSON.stringify({
        orderIdExternal: orderId,
        products: [
            {
                productIdExternal: 1354,
                name: "Produto 1",
                price: 15.9
            },
            {
                productIdExternal: 6599,
                name: "Produto 2",
                price: 102.99
            },
            {
                productIdExternal: 5536,
                name: "Produto 3",
                price: 66.59
            }
        ]
    });

    let params = {
        headers: {
            'Content-Type': 'application/json'
        }
    };

    let res = http.post('https://localhost:7137/api/Orders', payload, params);

    check(res, {
        'status is 200': (r) => r.status === 200
    });
}
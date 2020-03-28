const uri = 'api/BranchApi';
let branchess = [];

function getItems() {
    fetch(uri)
        .then(response => response.json())
        .then(data => _displayItems(data))
        .catch(error => console.error('Unable to get items.', error));
}

function _displayItems(data) {

  const tBody = document.getElementById('branchess');
    tBody.innerHTML = '';

    data.forEach(item => {
        
    let tr = tBody.insertRow();
    let td1 = tr.insertCell(0);
    
    var a = document.createElement('a');
    var name = document.createTextNode(item.branchName);
    a.appendChild(name);
    a.title = item.name;
    a.href = "Branch/Detail/" + item.id;
    td1.appendChild(a);

    let td2 = tr.insertCell(1);
    let time = document.createTextNode(item.isOpen ? 'OPEN' : 'CLOSED');
    td2.append(time);

    let td3 = tr.insertCell(2);
        let numberOfAssets = document.createTextNode(item.numberOfAssets);
        td3.appendChild(numberOfAssets);

    let td4 = tr.insertCell(3);
        let numberOfPatrons = document.createTextNode(item.numberOfPatrons);
        td4.appendChild(numberOfPatrons);
        
    });
    branchess = data;
    var table = jQuery(".tablesorter");
    table.tablesorter();
}
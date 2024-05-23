document.addEventListener('DOMContentLoaded', (event) => {
   
    const searchIcon = document.querySelector('.search-btn .header-search.search-toggle');

    
    searchIcon.addEventListener('click', () => {
       
        const searchBtn = document.querySelector('.search-btn');


        searchBtn.classList.toggle('open');
    });
});
document.getElementById("searchItem").addEventListener("keyup", function (elm) {
    elm.preventDefault();
    let searchedItem = document.getElementById("searched").value.toUpperCase();

    if (searchedItem.length > 0) {
        let searchDiv = document.getElementById("searchD")

        fetch(`/course/SearchLayout?s=${searchedItem}`)
            .then(res => res.json())
            .then(data => {
                searchDiv.innerHTML = "";
                data.forEach(elem => {
                    let listItem = DivElem(elem);
                    searchDiv.innerHTML += listItem;
                })
            })
    }
    else {
        document.getElementById("searchD").innerHTML = "";
    }
})

function DivElem(data) {
    let div = `
                <div class="search-result" style="display: flex; align-items: center; margin-bottom: 10px;">
                    <div class="search-result-image" style="margin-right: 10px;">
                        <a href="/Course/Detail/${data.id}">
                            <img src="/uploads/course/${data.img}" style="width:70px; height:70px;">
                        </a>
                    </div>
                    <div class="search-result-text">
                        <a href="/Course/Detail/${data.id}" style="color:orange;">
                            ${data.name}
                        </a>
                    </div>
                </div>`;
    return div;
}

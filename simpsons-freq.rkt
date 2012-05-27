#lang racket

(require lib/time)

(define raw
  (drop ;; because we know first 5 lines are empty or comment
   (call-with-input-file "simpsons_word_frequency.txt"
     (λ (in) (port->lines in))) 5))

(define (extract-words input)
  (cond [(empty? input) '()]
        [else
         (let ([m (regexp-match "(?<= )[a-z]+(?= )" (first input))])
           (if m 
               (cons (first m) (extract-words (rest input)))
               (extract-words (rest input))))]))

(define (extract-freqs input)
  (cond [(empty? input) '()]
        [(regexp-match "(?<= )[a-z]+(?= )" (first input))
         (cons (string->number (first (regexp-match "(?<=\\()[0-9]+" (first input))))
               (extract-freqs (rest input)))]
        [else (extract-freqs (rest input))]))

(define words (extract-words raw))
(define freqs (extract-freqs raw))

(define dict
  (make-hash
   (map list 
        words 
        freqs)))

(define sublists  
  (for/list ([i (in-range 1 16)]) ;; balls to efficiency   
    (filter (λ (x) (= i (string-length x))) words)))

(call-with-output-file "cs-jagged-array.txt" #:exists 'replace
  (λ (out)
    (for ([i (in-range (length sublists))])
      (display (format "words[~a] = new string[]{" i) out)
      (display (format "\"~a\"" (string-join (list-ref sublists i) "\", \"")) out)
      (display (format "};\t~n") out))))